using BusinessObject.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository.Interfaces;
using Service.Helpers;


namespace Service.BackgroundJobs
{
    public class BookingMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public BookingMonitorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckBookingsStatusAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[BookingMonitor] Lỗi: {ex.Message}");
                }

                // Chờ 5 phút rồi kiểm tra lại
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task CheckBookingsStatusAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var bookingRepo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

            var now = DateTimeHelper.NowVietnamTime();
            var allBookings = await bookingRepo.GetAllAsync();

            int cancelCount = 0, overtimeCount = 0;

            foreach (var booking in allBookings)
            {
                // --- 1️⃣ Tự động hủy nếu quá 15 phút mà chưa check-in ---
                if (booking.Status == BookingStatus.Booked && now > booking.StartTime.AddMinutes(15))
                {
                    booking.Status = BookingStatus.Cancelled;
                    booking.UpdatedAt = now;
                    await bookingRepo.UpdateAsync(booking);
                    cancelCount++;
                    continue;
                }

                // --- 2️⃣ Tự động chuyển sang OVERTIME nếu quá EndTime mà vẫn đang InUse ---
                if (booking.Status == BookingStatus.InUse && now > booking.EndTime)
                {
                    booking.Status = BookingStatus.Overtime;
                    booking.UpdatedAt = now;
                    await bookingRepo.UpdateAsync(booking);
                    overtimeCount++;
                }
            }

            if (cancelCount > 0 || overtimeCount > 0)
            {
                Console.WriteLine($"[BookingMonitor] Cập nhật: {cancelCount} booking bị huỷ, {overtimeCount} bị overtime.");
            }
        }
    }
}
