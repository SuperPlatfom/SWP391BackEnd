
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;

namespace Service.BackgroundJobs
{
    public class QuotaCarryOverBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public QuotaCarryOverBackgroundService(IServiceProvider serviceProvider)
        {
             _serviceProvider = serviceProvider;
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
            using var scope = _serviceProvider.CreateScope();
            var usageQuotaService = _serviceProvider.GetRequiredService<IUsageQuotaService>;
            var usageQuotaRepository = _serviceProvider.GetRequiredService<IUsageQuotaRepository>;

            var vietnamNow = DateTimeHelper.ToVietnamTime(DateTime.UtcNow);

            var previousWeekStartVN = DateTimeHelper.GetWeekStartDate(vietnamNow).AddDays(-7);
        var currentWeekStartVN = DateTimeHelper.GetWeekStartDate(vietnamNow);

        var previousWeekStartUtc = DateTimeHelper.ToUtcFromVietnamTime(previousWeekStartVN);
        var currentWeekStartUtc = DateTimeHelper.ToUtcFromVietnamTime(currentWeekStartVN);

        var oldQuotas = await usageQuotaRepository().GetAllAsync(q => q.WeekStartDate == previousWeekStartUtc);
        if (oldQuotas == null || !oldQuotas.Any())
            return;

        foreach (var oldQuota in oldQuotas)
        {
               
            await usageQuotaService().EnsureQuotaExistsAsync(oldQuota.AccountId, oldQuota.GroupId, oldQuota.VehicleId, currentWeekStartUtc);
                var newQuota = await usageQuotaRepository().GetUsageQuotaAsync(oldQuota.AccountId, oldQuota.GroupId, oldQuota.VehicleId, currentWeekStartUtc);

            if (newQuota != null)
            {
                   
                newQuota.HoursUsed += oldQuota.HoursAdvance;
                    var calculatingDebt = (oldQuota.HoursDebt + oldQuota.HoursUsed) - oldQuota.HoursLimit;
                    if (calculatingDebt > 0)
                    {
                        newQuota.HoursDebt += calculatingDebt;
                        oldQuota.HoursDebt -= calculatingDebt;
                    }
               
                newQuota.LastUpdated = DateTime.UtcNow;

            
                oldQuota.HoursAdvance = 0;
                oldQuota.LastUpdated = DateTime.UtcNow;

                await usageQuotaRepository().UpdateAsync(newQuota);
                await usageQuotaRepository().UpdateAsync(oldQuota);
               
                }
        }

        await usageQuotaRepository().SaveChangesAsync();
    }
    }
}
