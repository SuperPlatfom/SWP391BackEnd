﻿using BusinessObject.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;


namespace Service.BackgroundJobs
{
    public class BookingMonitorBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public BookingMonitorBackgroundService(IServiceProvider serviceProvider)
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

          
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task CheckBookingsStatusAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var bookingRepo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
            var usageQuotaRepo = scope.ServiceProvider.GetRequiredService<IUsageQuotaRepository>();
            var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

            var now = DateTimeHelper.NowVietnamTime();
            var allBookings = await bookingRepo.GetAllAsync();

        

            foreach (var booking in allBookings)
            {
                // --- 1️⃣ Tự động hủy nếu quá 15 phút mà chưa check-in ---
                if (booking.Status == BookingStatus.Booked && DateTime.UtcNow >= booking.StartTime.AddMinutes(15))
                {
                    await bookingService.CancelBookingAsync(booking.Id);
                    continue;
                }

                // --- 2️⃣ Tự động chuyển sang OVERTIME nếu quá EndTime mà vẫn đang InUse ---
                if (booking.Status == BookingStatus.InUse && DateTime.UtcNow > booking.EndTime)
                {
                    booking.Status = BookingStatus.Overtime;
                    booking.UpdatedAt = DateTime.UtcNow;
                    await bookingRepo.UpdateAsync(booking);
                    await bookingRepo.SaveChangesAsync();
                
                }
            }

            var overtimeBookings = allBookings.Where(b => b.Status == BookingStatus.Overtime).ToList();

            foreach (var overtime in overtimeBookings)
            {
                var overlappingBookings = allBookings.Where(b =>
                    b.VehicleId == overtime.VehicleId &&
                    b.Id != overtime.Id &&
                    b.StartTime < overtime.EndTime && // bị trùng thời gian
                    b.Status == BookingStatus.Booked
                ).ToList();

                foreach (var nextBooking in overlappingBookings)
                {
                    Console.WriteLine($"[BookingMonitor] Booking {nextBooking.Id} bị hủy do trùng với booking overtime {overtime.Id}.");

                    // ✅ Hoàn lại toàn bộ giờ cho người dùng booking sau
                    var weekStartUtc = DateTimeHelper.GetWeekStartDate(nextBooking.StartTime);
                    var quota = await usageQuotaRepo.GetUsageQuotaAsync(nextBooking.UserId, nextBooking.GroupId, nextBooking.VehicleId, weekStartUtc);

                    if (quota != null)
                    {
                        var durationHours = (decimal)(nextBooking.EndTime - nextBooking.StartTime).TotalHours;
                        quota.HoursUsed -= durationHours;
                        if (quota.HoursUsed < 0) quota.HoursUsed = 0;

                        quota.LastUpdated = DateTime.UtcNow;
                        await usageQuotaRepo.UpdateAsync(quota);
                    }

                    // Cập nhật trạng thái booking sau
                    nextBooking.Status = BookingStatus.Cancelled;
                    nextBooking.UpdatedAt = DateTime.UtcNow;
                    await bookingRepo.UpdateAsync(nextBooking);
                }

                await usageQuotaRepo.SaveChangesAsync();
                await bookingRepo.SaveChangesAsync();
            }


        }
    }
}
