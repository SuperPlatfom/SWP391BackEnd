

using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;
using System.Security.Claims;

namespace Service
{
    public class UsageQuotaService : IUsageQuotaService
    {
        private readonly IUsageQuotaRepository _quotaRepo;

        public UsageQuotaService(IUsageQuotaRepository quotaRepo)
        {
            _quotaRepo = quotaRepo;
        }

        public async Task<(bool IsSuccess, string Message, object? Data)> GetRemainingQuotaAsync(Guid groupId, Guid vehicleId, ClaimsPrincipal user)
        {

            var userId = GetUserId(user);
            if (userId == Guid.Empty)
                return (false, "Không xác định được người dùng.", null);


            var vietnamNow = DateTimeHelper.ToVietnamTime(DateTime.UtcNow);
            var weekStartVietnam = GetWeekStartDate(vietnamNow);
            var weekStartUtc = DateTime.SpecifyKind(
            DateTimeHelper.ToUtcFromVietnamTime(weekStartVietnam),
            DateTimeKind.Utc
            );

            var quota = await _quotaRepo.GetUsageQuotaAsync(userId, groupId, vehicleId, weekStartUtc);
            if (quota == null)
            {
                var rateInfo = await _quotaRepo.GetQuotaRateAsync(userId, vehicleId);
                if (rateInfo == null)
                    return (false, "Không tìm thấy thông tin sở hữu hoặc xe.", null);

                var (weeklyQuotaHours, ownershipRate) = rateInfo.Value;
                var safeOwnershipRate = ownershipRate ?? 0m;
                var hoursLimit = weeklyQuotaHours * safeOwnershipRate / 100m;
                quota = new UsageQuota
                {
                    Id = Guid.NewGuid(),
                    AccountId = userId,
                    GroupId = groupId,
                    VehicleId = vehicleId,
                    WeekStartDate = weekStartUtc,
                    HoursLimit = hoursLimit,
                    HoursUsed = 0,
                    LastUpdated = DateTime.UtcNow
                };

                await _quotaRepo.AddAsync(quota);
                await _quotaRepo.SaveChangesAsync();
            }


            decimal remaining = quota.HoursLimit - quota.HoursUsed;

            var data = new UsageQuotaResponseModel
            {
                VehicleId = quota.VehicleId,
                GroupId = quota.GroupId,
                WeekStartDate = quota.WeekStartDate,
                HoursLimit = quota.HoursLimit,
                HoursUsed = quota.HoursUsed,
                RemainingHours = remaining
            };

            string message = remaining <= 0
             ? "Bạn đã hết quota tuần này."
             : $"Bạn còn {remaining:F2} giờ quota.";

            return (true, message, data);
        }

        // 🧩 Helper: Lấy userId từ Claims
        private Guid GetUserId(ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }

        // 🧩 Helper: Tính ngày bắt đầu tuần (Thứ Hai)
        private DateTime GetWeekStartDate(DateTime vietnamNow)
        {
            int diff = (7 + (vietnamNow.DayOfWeek - DayOfWeek.Monday)) % 7;
            return vietnamNow.Date.AddDays(-diff);
        }
    }
}
