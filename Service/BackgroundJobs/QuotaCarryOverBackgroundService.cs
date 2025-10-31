
using Microsoft.Extensions.Hosting;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;

namespace Service.BackgroundJobs
{
    public class QuotaCarryOverBackgroundService : BackgroundService
    {
        private readonly IUsageQuotaRepository _usageQuotaRepository;
        private readonly IUsageQuotaService _usageQuotaService;

        public QuotaCarryOverBackgroundService(IUsageQuotaRepository usageQuotaRepository, IUsageQuotaService usageQuotaService)
        {
            _usageQuotaRepository = usageQuotaRepository;
            _usageQuotaService = usageQuotaService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Chạy vòng lặp vô hạn, dừng khi token bị hủy (ứng dụng tắt)
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var nowVN = DateTimeHelper.ToVietnamTime(DateTime.UtcNow);

                    // Nếu là thứ Hai (start of week) và giờ là 00:00 ± 10 phút
                    if (nowVN.DayOfWeek == DayOfWeek.Monday &&
                        nowVN.Hour == 0 && nowVN.Minute <= 10)
                    {
                        await CarryOverQuotaAsync();
                    }

                    // Kiểm tra lại mỗi 10 phút
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }
                catch (Exception)
                {
                    
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
private async Task CarryOverQuotaAsync()
    {
            // Lấy danh sách quota của tuần cũ (tuần vừa kết thúc)
            var vietnamNow = DateTimeHelper.ToVietnamTime(DateTime.UtcNow);

            var previousWeekStartVN = DateTimeHelper.GetWeekStartDate(vietnamNow).AddDays(-7);
        var currentWeekStartVN = DateTimeHelper.GetWeekStartDate(vietnamNow);

        var previousWeekStartUtc = DateTimeHelper.ToUtcFromVietnamTime(previousWeekStartVN);
        var currentWeekStartUtc = DateTimeHelper.ToUtcFromVietnamTime(currentWeekStartVN);

        var oldQuotas = await _usageQuotaRepository.GetAllAsync(q => q.WeekStartDate == previousWeekStartUtc);
        if (oldQuotas == null || !oldQuotas.Any())
            return;

        foreach (var oldQuota in oldQuotas)
        {
                // Lấy quota của tuần hiện tại
            await _usageQuotaService.EnsureQuotaExistsAsync(oldQuota.AccountId, oldQuota.GroupId, oldQuota.VehicleId, currentWeekStartUtc);
                var newQuota = await _usageQuotaRepository.GetUsageQuotaAsync(oldQuota.AccountId, oldQuota.GroupId, oldQuota.VehicleId, currentWeekStartUtc);

            if (newQuota != null)
            {
                    // Chuyển phần dư hoặc nợ sang tuần mới
                newQuota.HoursUsed += oldQuota.HoursAdvance;
                    var calculatingDebt = (oldQuota.HoursDebt + oldQuota.HoursUsed) - oldQuota.HoursLimit;
                    if (calculatingDebt > 0)
                    {
                        newQuota.HoursDebt += calculatingDebt;
                        oldQuota.HoursDebt -= calculatingDebt;
                    }
               
                newQuota.LastUpdated = DateTime.UtcNow;

                // Reset quota cũ để tránh cộng dồn nhiều lần
                oldQuota.HoursAdvance = 0;
                oldQuota.LastUpdated = DateTime.UtcNow;

                await _usageQuotaRepository.UpdateAsync(newQuota);
                await _usageQuotaRepository.UpdateAsync(oldQuota);
               
                }
        }

        await _usageQuotaRepository.SaveChangesAsync();
    }
    }
}
