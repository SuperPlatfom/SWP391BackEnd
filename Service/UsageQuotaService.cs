

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
          
            var existingQuota = await _quotaRepo.GetUsageQuotaAsync(userId, groupId, vehicleId, weekStartUtc);
            if (existingQuota != null)
            {
                return (true, "Quota đã tồn tại, không cần tạo lại.");
            }

          
            var rateInfo = await _quotaRepo.GetQuotaRateAsync(userId, vehicleId);
            if (rateInfo == null)
            {
                return (false, "Không tìm thấy thông tin sở hữu hoặc xe.");
            }

            var (weeklyQuotaHours, ownershipRate) = rateInfo.Value;
            var safeOwnershipRate = ownershipRate ?? 0m;

          
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
                HoursAdvance = 0,
                HoursDebt = 0,
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
            var result  = await _quotaRepo.GetQuotaRateAsync(userId,vehicleId);
     
            
                decimal weeklyQuotaHours = result.Value.weeklyQuotaHours;
            decimal ownershipRate = (decimal)result.Value.ownershipRate;
            
            
            var quota = await _quotaRepo.GetUsageQuotaAsync(userId, groupId, vehicleId, weekStartUtc);
            if (quota == null)
                return (false, "Không thể lấy quota sau khi tạo.", null);

            decimal remaining = quota.HoursLimit - quota.HoursUsed - quota.HoursDebt;
            if (remaining < 0) remaining = 0;

            // --- Tính giờ có thể đặt cho tuần sau ---
            decimal debtOverflow = 0; // phần nợ vượt qua limit tuần này
            if (quota.HoursUsed + quota.HoursDebt > quota.HoursLimit)
            {
                debtOverflow = (quota.HoursUsed + quota.HoursDebt) - quota.HoursLimit;
            }

            // Công thức: Giờ có thể đặt tuần sau = Limit - Advance - debtOverflow
            decimal remainingNextWeek = quota.HoursLimit - quota.HoursAdvance - debtOverflow;
            if (remainingNextWeek < 0) remainingNextWeek = 0;

            // Dữ liệu trả về
            var data = new UsageQuotaResponseModel
            {
                VehicleId = quota.VehicleId,
                GroupId = quota.GroupId,
                WeekStartDate = quota.WeekStartDate,
                HoursLimit = quota.HoursLimit,
                HoursUsed = quota.HoursUsed,
                HoursAdvance = quota.HoursAdvance,
                HoursDebt = quota.HoursDebt,
                RemainingHours = remaining,
                RemainingHoursNextWeek = remainingNextWeek,
                OwnershipRate = ownershipRate,
                WeeklyQuotaHours = weeklyQuotaHours,
            };

            // -------------------------
            // Format thời gian (hiển thị giờ & phút)
            // -------------------------
            int remainingHours = (int)Math.Floor(remaining);
            int remainingMinutes = (int)Math.Round((remaining - remainingHours) * 60);

            string formattedTime = remainingHours > 0 && remainingMinutes > 0
                ? $"{remainingHours} giờ {remainingMinutes} phút"
                : remainingHours > 0 ? $"{remainingHours} giờ" : $"{remainingMinutes} phút";

            int nextHours = (int)Math.Floor(remainingNextWeek);
            int nextMinutes = (int)Math.Round((remainingNextWeek - nextHours) * 60);

            string formattedNextWeek = nextHours > 0 && nextMinutes > 0
                ? $"{nextHours} giờ {nextMinutes} phút"
                : nextHours > 0 ? $"{nextHours} giờ" : $"{nextMinutes} phút";

            // -------------------------
            // Tạo thông báo kết quả
            // -------------------------
            string message;
            if (remaining <= 0 && remainingNextWeek <= 0)
            {
                message = "Bạn đã hết giờ đặt lịch cho tuần này và tuần sau.";
            }
            else if (remaining <= 0)
            {
                message = $"Bạn đã hết giờ đặt lịch cho tuần này. Tuần sau bạn còn {formattedNextWeek} để đặt trước.";
            }
            else
            {
                message = $"Bạn còn {formattedTime} để đặt trong tuần này, và {formattedNextWeek} để đặt trước cho tuần sau.";
            }

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

       
       
    }
}
