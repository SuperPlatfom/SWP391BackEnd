

using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Enums;
using BusinessObject.Models;
using Repository;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;
using System.Security.Claims;

namespace Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo;

        public BookingService(IBookingRepository bookingRepo)
        {
            _bookingRepo = bookingRepo;
        }

        public async Task<(bool IsSuccess, string Message, BookingResponseModel? Data)> CreateBookingAsync(BookingRequestModel request, ClaimsPrincipal user)
        {
            var now = DateTimeHelper.NowVietnamTime();
            var userId = GetUserId(user);

            if (request.StartTime < now.AddMinutes(30))
                return (false, "Thời gian bắt đầu phải cách hiện tại ít nhất 30 phút.", null);

            if (request.EndTime <= request.StartTime)
                return (false, "Thời gian kết thúc phải sau thời gian bắt đầu.", null);

            var existingBookings = await _bookingRepo.GetBookingsByVehicleAsync(request.VehicleId);

            foreach (var b in existingBookings)
            {
                if (b.Status == BookingStatus.Cancelled || b.Status == BookingStatus.Completed) continue;

                var existingStart = b.StartTime.AddMinutes(-30);
                var existingEnd = b.EndTime.AddMinutes(30);

                bool isOverlap =
                    request.StartTime < existingEnd &&
                    request.EndTime > existingStart;

                if (isOverlap)
                {
                    return (false,
                        $"Xe này đã có người đặt từ {b.StartTime:HH:mm} đến {b.EndTime:HH:mm}. " +
                        $"Vui lòng chọn thời gian cách ít nhất 30 phút.",
                        null);
                }
            }

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                GroupId = request.GroupId,
                VehicleId = request.VehicleId,
                UserId = userId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Status = BookingStatus.Booked,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _bookingRepo.AddAsync(booking);
            return (true, "Đặt lịch thành công.", MapToResponse(booking));
        }

        public async Task<(bool IsSuccess, string Message)> CancelBookingAsync(Guid bookingId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
                return (false, "Không tìm thấy lịch đặt.");

            if (booking.Status == BookingStatus.Cancelled)
                return (false, "Lịch này đã bị hủy trước đó.");

            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTimeHelper.NowVietnamTime();

            await _bookingRepo.UpdateAsync(booking);
            return (true, "Hủy lịch thành công.");
        }

        public async Task<(bool IsSuccess, string Message)> CheckInAsync(Guid bookingId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
                return (false, "Không tìm thấy lịch đặt.");

            if (booking.Status != BookingStatus.Booked)
                return (false, "Chỉ có thể check-in lịch đang ở trạng thái Booked.");

            booking.Status = BookingStatus.InUse;
            booking.UpdatedAt = DateTimeHelper.NowVietnamTime();

            await _bookingRepo.UpdateAsync(booking);
            return (true, "Check-in thành công.");
        }

        public async Task<(bool IsSuccess, string Message)> CheckOutAsync(Guid bookingId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
                return (false, "Không tìm thấy lịch đặt.");

            if (booking.Status != BookingStatus.InUse)
                return (false, "Chỉ có thể check-out lịch đã check-in.");

            booking.Status = BookingStatus.Completed;
            booking.UpdatedAt = DateTimeHelper.NowVietnamTime();

            await _bookingRepo.UpdateAsync(booking);
            return (true, "Check-out thành công.");
        }

        public async Task<(bool IsSuccess, string Message, List<BookingResponseModel>? Data)>
            GetBookingsByGroupAndVehicleAsync(Guid groupId, Guid vehicleId)
        {
            var bookings = await _bookingRepo.GetBookingsByVehicleInGroupAsync(groupId, vehicleId);

            if (!bookings.Any())
                return (false, "Không có lịch đặt nào cho xe này trong nhóm.", null);

            var data = bookings.Select(MapToResponse).ToList();

            return (true, "Lấy danh sách lịch đặt của nhóm thành công.", data);
        }

        private Guid GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng trong token.");
            return Guid.Parse(userIdClaim);
        }
        private static BookingResponseModel MapToResponse(Booking booking)
        {
            return new BookingResponseModel
            {
                Id = booking.Id,
                GroupId = booking.GroupId,
                VehicleId = booking.VehicleId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Purpose = booking.Purpose,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt,
                //VehicleName = booking.Vehicle?.Model,
              //  GroupName = booking.Group?.Name,
              //  UserName = booking.User?.FullName
            };
        }
    }
}