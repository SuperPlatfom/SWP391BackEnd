

using BusinessObject.Models;
using Repository;
using Service.Helpers;
using Service.Interfaces;

namespace Service
{
    public class BookingService : IBookingService
    {
        private readonly BookingRepository _bookingRepo;

        public BookingService(BookingRepository bookingRepo)
        {
            _bookingRepo = bookingRepo;
        }

        public async Task<(bool IsSuccess, string Message, Booking? Data)> CreateBookingAsync(Booking booking)
        {
            var now = DateTimeHelper.NowVietnamTime();

            if (booking.StartTime < now)
                return (false, "Không thể đặt lịch trong quá khứ.", null);

            if ((booking.StartTime - now).TotalMinutes < 30)
                return (false, "Thời gian đặt lịch phải cách hiện tại ít nhất 30 phút.", null);

            booking.Id = Guid.NewGuid();
            booking.Status = "Pending";
            booking.CreatedAt = now;
            booking.UpdatedAt = now;

            await _bookingRepo.AddAsync(booking);
            return (true, "Đặt lịch thành công.", booking);
        }

        public async Task<(bool IsSuccess, string Message)> CancelBookingAsync(Guid bookingId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
                return (false, "Không tìm thấy lịch đặt.");

            if (booking.Status == "Cancelled")
                return (false, "Lịch này đã bị hủy trước đó.");

            booking.Status = "Cancelled";
            booking.UpdatedAt = DateTimeHelper.NowVietnamTime();

            await _bookingRepo.UpdateAsync(booking);
            return (true, "Hủy lịch thành công.");
        }

        public async Task<(bool IsSuccess, string Message)> CheckInAsync(Guid bookingId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
                return (false, "Không tìm thấy lịch đặt.");

            if (booking.Status != "Pending")
                return (false, "Chỉ có thể check-in lịch đang ở trạng thái Pending.");

            booking.Status = "CheckedIn";
            booking.UpdatedAt = DateTimeHelper.NowVietnamTime();

            await _bookingRepo.UpdateAsync(booking);
            return (true, "Check-in thành công.");
        }

        public async Task<(bool IsSuccess, string Message)> CheckOutAsync(Guid bookingId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
                return (false, "Không tìm thấy lịch đặt.");

            if (booking.Status != "CheckedIn")
                return (false, "Chỉ có thể check-out lịch đã check-in.");

            booking.Status = "Completed";
            booking.UpdatedAt = DateTimeHelper.NowVietnamTime();

            await _bookingRepo.UpdateAsync(booking);
            return (true, "Check-out thành công.");
        }
    }
}
