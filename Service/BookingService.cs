

using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Enums;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;
using System.Security.Claims;

namespace Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IUsageQuotaService _usageQuotaService;
        private readonly IUsageQuotaRepository _usageQuotaRepository;

        public BookingService(IBookingRepository bookingRepo, IUsageQuotaService usageQuotaService, IUsageQuotaRepository usageQuotaRepository)
        {
            _bookingRepo = bookingRepo;
            _usageQuotaService = usageQuotaService;
            _usageQuotaRepository = usageQuotaRepository;
        }

        public async Task<(bool IsSuccess, string Message, BookingResponseModel? Data)> CreateBookingAsync(BookingRequestModel request, ClaimsPrincipal user)
        {
        
            var userId = GetUserId(user);

            var startUtc = DateTimeHelper.ToUtcFromVietnamTime(request.StartTime);
            var endUtc = DateTimeHelper.ToUtcFromVietnamTime(request.EndTime);

            if (startUtc < DateTime.UtcNow.AddMinutes(30))
                return (false, "Thời gian bắt đầu phải cách hiện tại ít nhất 30 phút.", null);
            if (endUtc <= startUtc)
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
                $"Xe này đã có người đặt từ {DateTimeHelper.ToVietnamTime(b.StartTime):HH:mm} đến {DateTimeHelper.ToVietnamTime(b.EndTime):HH:mm}. " +
                $"Vui lòng chọn thời gian cách ít nhất 30 phút.",
                null);
                }
            }

            var bookingHours = (decimal)(endUtc - startUtc).TotalHours;
            var vietnamNow = DateTimeHelper.ToVietnamTime(DateTime.UtcNow);
            var weekStartVietnam = DateTimeHelper.GetWeekStartDate(vietnamNow);
            var weekStartUtc = DateTime.SpecifyKind(
                DateTimeHelper.ToUtcFromVietnamTime(weekStartVietnam),
                DateTimeKind.Utc
            );

            var ensureResult = await _usageQuotaService.EnsureQuotaExistsAsync(userId, request.GroupId, request.VehicleId, weekStartUtc);
            if (!ensureResult.IsSuccess)
                return (false, ensureResult.Message, null);

            var quota = await _usageQuotaRepository.GetUsageQuotaAsync(userId, request.GroupId, request.VehicleId, weekStartUtc);
            if (quota == null)
                return (false, "Không thể lấy thông tin quota.", null);

            var remaining = quota.HoursLimit - quota.HoursUsed;
            if (bookingHours > remaining)
            {
                return (false, $"Thời gian đặt lịch tuần này của bạn không đủ. Thời gian còn lại: {remaining:F2} giờ.", null);
            }

            quota.HoursUsed += bookingHours;
            quota.LastUpdated = DateTime.UtcNow;
            await _usageQuotaRepository.UpdateAsync(quota);
            await _usageQuotaRepository.SaveChangesAsync();

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                GroupId = request.GroupId,
                VehicleId = request.VehicleId,
                UserId = userId,
                StartTime = startUtc,
                EndTime = endUtc,
                Status = BookingStatus.Booked,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _bookingRepo.AddAsync(booking);
            await _bookingRepo.SaveChangesAsync();
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
            booking.UpdatedAt = DateTime.UtcNow;

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
            booking.UpdatedAt = DateTime.UtcNow;

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

        public async Task<(bool IsSuccess, string Message, BookingResponseModel? Data)>
    UpdateBookingAsync(BookingUpdateRequestModel request, ClaimsPrincipal user)
        {
            var booking = await _bookingRepo.GetByIdAsync(request.Id);
            if (booking == null)
                return (false, "Không tìm thấy lịch đặt.", null);

            // Chỉ cho phép update khi status là BOOKED hoặc INUSE
            if (booking.Status != BookingStatus.Booked && booking.Status != BookingStatus.InUse)
                return (false, "Chỉ có thể cập nhật lịch ở trạng thái BOOKED hoặc INUSE.", null);

            var nowVN = DateTimeHelper.NowVietnamTime();

            if (request.EndTime <= request.StartTime)
                return (false, "Thời gian kết thúc phải sau thời gian bắt đầu.", null);

            // Chuyển về UTC để lưu DB
            var newStartUtc = DateTimeHelper.ToUtcFromVietnamTime(request.StartTime);
            var newEndUtc = DateTimeHelper.ToUtcFromVietnamTime(request.EndTime);

            // Lấy danh sách booking khác của cùng xe để kiểm tra overlap
            var existingBookings = await _bookingRepo.GetBookingsByVehicleAsync(booking.VehicleId);

            // Lọc bỏ chính booking đang update
            existingBookings = existingBookings
                .Where(b => b.Id != booking.Id &&
                            b.Status != BookingStatus.Cancelled &&
                            b.Status != BookingStatus.Completed)
                .ToList();

            // ----------------------------
            // CASE 1: Status = INUSE
            // ----------------------------
            if (booking.Status == BookingStatus.InUse)
            {
                if (newStartUtc != booking.StartTime)
                    return (false, "Không thể thay đổi thời gian bắt đầu khi xe đang được sử dụng.", null);

                if (newEndUtc <= booking.StartTime)
                    return (false, "Thời gian kết thúc phải sau thời gian bắt đầu.", null);

     
                foreach (var b in existingBookings)
                {
                    bool isOverlap = newEndUtc > b.StartTime && booking.StartTime < b.EndTime;
                    if (isOverlap)
                    {
                        return (false,
                            $"Thời gian kết thúc mới ({DateTimeHelper.ToVietnamTime(newEndUtc):HH:mm}) " +
                            $"bị trùng với lịch đặt khác từ {DateTimeHelper.ToVietnamTime(b.StartTime):HH:mm} " +
                            $"đến {DateTimeHelper.ToVietnamTime(b.EndTime):HH:mm}.",
                            null);
                    }
                }

                var userId = booking.UserId;
                var groupId = booking.GroupId;
                var vehicleId = booking.VehicleId;

                var vietnamNow = DateTimeHelper.ToVietnamTime(DateTime.UtcNow);
                var weekStartVietnam = DateTimeHelper.GetWeekStartDate(vietnamNow);
                var weekStartUtc = DateTime.SpecifyKind(
                    DateTimeHelper.ToUtcFromVietnamTime(weekStartVietnam),
                    DateTimeKind.Utc
                );

                var quota = await _usageQuotaRepository.GetUsageQuotaAsync(userId, groupId, vehicleId, weekStartUtc);
                if (quota != null)
                {
                    var oldDuration = (decimal)(booking.EndTime - booking.StartTime).TotalHours;
                    var newDuration = (decimal)(newEndUtc - booking.StartTime).TotalHours;
                    var deltaHours = newDuration - oldDuration;

                    if (deltaHours > 0 && quota.HoursUsed + deltaHours > quota.HoursLimit)
                        return (false, $"Không đủ quota để kéo dài thời gian. Bạn còn {quota.HoursLimit - quota.HoursUsed:F2} giờ.", null);

                    quota.HoursUsed += deltaHours;
                    quota.LastUpdated = DateTime.UtcNow;
                    await _usageQuotaRepository.UpdateAsync(quota);
                    await _usageQuotaRepository.SaveChangesAsync();
                }

                booking.EndTime = newEndUtc;
                booking.UpdatedAt = DateTime.UtcNow;
                await _bookingRepo.UpdateAsync(booking);

                return (true, "Cập nhật thời gian kết thúc thành công.", MapToResponse(booking));
            }

            // ----------------------------
            // CASE 2: Status = BOOKED
            // ----------------------------
            if (booking.Status == BookingStatus.Booked)
            {
                if (request.StartTime < nowVN.AddMinutes(30))
                    return (false, "Thời gian bắt đầu phải cách hiện tại ít nhất 30 phút.", null);

                foreach (var b in existingBookings)
                {

                    var existingStart = b.StartTime.AddMinutes(-30);
                    var existingEnd = b.EndTime.AddMinutes(30);

                    bool isOverlap = newStartUtc < existingEnd && newEndUtc > existingStart;

                    if (isOverlap)
                    {
                        return (false,
                            $"Xe này đã có người đặt từ {DateTimeHelper.ToVietnamTime(b.StartTime):HH:mm} " +
                            $"đến {DateTimeHelper.ToVietnamTime(b.EndTime):HH:mm}. " +
                            $"Vui lòng chọn thời gian cách ít nhất 30 phút.",
                            null);
                    }
                }

                var userId = booking.UserId;
                var groupId = booking.GroupId;
                var vehicleId = booking.VehicleId;

                var vietnamNow = DateTimeHelper.ToVietnamTime(DateTime.UtcNow);
                var weekStartVietnam = DateTimeHelper.GetWeekStartDate(vietnamNow);
                var weekStartUtc = DateTime.SpecifyKind(
                    DateTimeHelper.ToUtcFromVietnamTime(weekStartVietnam),
                    DateTimeKind.Utc
                );

                var quota = await _usageQuotaRepository.GetUsageQuotaAsync(userId, groupId, vehicleId, weekStartUtc);
                if (quota != null)
                {
                    var oldDuration = (decimal)(booking.EndTime - booking.StartTime).TotalHours;
                    var newDuration = (decimal)(newEndUtc - newStartUtc).TotalHours;
                    var deltaHours = newDuration - oldDuration;

                    if (deltaHours > 0 && quota.HoursUsed + deltaHours > quota.HoursLimit)
                        return (false, $"Không đủ quota để cập nhật lịch. Bạn còn {quota.HoursLimit - quota.HoursUsed:F2} giờ.", null);

                    quota.HoursUsed += deltaHours;
                    quota.LastUpdated = DateTime.UtcNow;
                    await _usageQuotaRepository.UpdateAsync(quota);
                    await _usageQuotaRepository.SaveChangesAsync();
                }

                booking.StartTime = newStartUtc;
                booking.EndTime = newEndUtc;
                booking.UpdatedAt = DateTime.UtcNow;
                await _bookingRepo.UpdateAsync(booking);

                return (true, "Cập nhật lịch đặt thành công.", MapToResponse(booking));
            }

            return (false, "Không thể cập nhật lịch đặt.", null);
        }

        private static BookingResponseModel MapToResponse(Booking booking)
        {
            return new BookingResponseModel
            {
                Id = booking.Id,
                GroupId = booking.GroupId,
                VehicleId = booking.VehicleId,
                StartTime = DateTimeHelper.ToVietnamTime(booking.StartTime),
                EndTime = DateTimeHelper.ToVietnamTime(booking.EndTime),
                Purpose = booking.Purpose,
                Status = booking.Status,
                CreatedAt = DateTimeHelper.ToVietnamTime(booking.CreatedAt),
                UpdatedAt = DateTimeHelper.ToVietnamTime(booking.UpdatedAt)
            };
        }
    }
}