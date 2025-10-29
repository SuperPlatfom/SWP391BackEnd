

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

        public async Task<(bool IsSuccess, string Message)> EnsureQuotaExistsAsync(Guid userId, Guid groupId, Guid vehicleId, DateTime weekStartUtc)
        {
            // 🧩 Kiểm tra quota đã tồn tại chưa
            var existingQuota = await _quotaRepo.GetUsageQuotaAsync(userId, groupId, vehicleId, weekStartUtc);
            if (existingQuota != null)
            {
                return (true, "Quota đã tồn tại, không cần tạo lại.");
            }

            // 🧩 Lấy thông tin rate
            var rateInfo = await _quotaRepo.GetQuotaRateAsync(userId, vehicleId);
            if (rateInfo == null)
            {
                return (false, "Không tìm thấy thông tin sở hữu hoặc xe.");
            }

            var (weeklyQuotaHours, ownershipRate) = rateInfo.Value;
            var safeOwnershipRate = ownershipRate ?? 0m;

            // 🧩 Tính quota giờ
            var hoursLimit = weeklyQuotaHours * safeOwnershipRate / 100m;

            var quota = new UsageQuota
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

            return (true, $"Đã tạo quota mới cho xe {vehicleId} với {hoursLimit:F2} giờ.");
        }

        public async Task<(bool IsSuccess, string Message, object? Data)> GetRemainingQuotaAsync(Guid groupId, Guid vehicleId, ClaimsPrincipal user)
        {

            var userId = GetUserId(user);
            if (userId == Guid.Empty)
                return (false, "Không xác định được người dùng.", null);


            var vietnamNow = DateTimeHelper.ToVietnamTime(DateTime.UtcNow);
            var weekStartVietnam = DateTimeHelper.GetWeekStartDate(vietnamNow);
            var weekStartUtc = DateTime.SpecifyKind(
            DateTimeHelper.ToUtcFromVietnamTime(weekStartVietnam),
            DateTimeKind.Utc
            );

            var ensureResult = await EnsureQuotaExistsAsync(userId, groupId, vehicleId, weekStartUtc);
            if (!ensureResult.IsSuccess)
                return (false, ensureResult.Message, null);

            var quota = await _quotaRepo.GetUsageQuotaAsync(userId, groupId, vehicleId, weekStartUtc);
            if (quota == null)
                return (false, "Không thể lấy quota sau khi tạo.", null);

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

        /// <summary>
        /// Gọi repo để reset quota
        /// </summary>
        public async Task ResetWeeklyQuotaAsync()
        {
            await _quotaRepo.ResetAllQuotaHoursUsedAsync();
        }

        private Guid GetUserId(ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }

        // 🧩 Helper: Tính ngày bắt đầu tuần (Thứ Hai)
       
    }
}
