

using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Enums;
using BusinessObject.Models;
using Google.Apis.Storage.v1;
using Microsoft.AspNetCore.Http;
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
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly ITripEventRepository _tripEventRepository;

        public BookingService(IBookingRepository bookingRepo, IUsageQuotaService usageQuotaService, IUsageQuotaRepository usageQuotaRepository, IFirebaseStorageService firebaseStorageService, ITripEventRepository tripEventRepository)
        {
            _bookingRepo = bookingRepo;
            _usageQuotaService = usageQuotaService;
            _usageQuotaRepository = usageQuotaRepository;
            _firebaseStorageService = firebaseStorageService;
            _tripEventRepository = tripEventRepository;

        }

        public async Task<(bool IsSuccess, string Message, BookingResponseModel? Data)> CreateBookingAsync(BookingRequestModel request, ClaimsPrincipal user)
        {
        
            var userId = GetUserId(user);

            var startUtc = DateTimeHelper.ToUtcFromVietnamTime(request.StartTime);
            var endUtc = DateTimeHelper.ToUtcFromVietnamTime(request.EndTime);

            if (startUtc < DateTime.UtcNow.AddMinutes(15))
                return (false, "Thời gian bắt đầu phải cách hiện tại ít nhất 15 phút.", null);
            if (endUtc <= startUtc)
                return (false, "Thời gian kết thúc phải sau thời gian bắt đầu.", null);

            var existingBookings = await _bookingRepo.GetBookingsByVehicleAsync(request.VehicleId);

            foreach (var b in existingBookings)
            {
                if (b.Status == BookingStatus.Cancelled || b.Status == BookingStatus.Completed) continue;

                var existingStartVN = DateTimeHelper.ToVietnamTime(b.StartTime.AddMinutes(-30));
                var existingEndVN = DateTimeHelper.ToVietnamTime(b.EndTime.AddMinutes(30));

                bool isOverlap =
                    request.StartTime < existingEndVN &&
                    request.EndTime > existingStartVN;

                if (isOverlap)
                {
                    return (false,
                $"Xe này đã có người đặt từ {DateTimeHelper.ToVietnamTime(b.StartTime):HH:mm} đến {DateTimeHelper.ToVietnamTime(b.EndTime):HH:mm}. " +
                $"Vui lòng chọn thời gian cách ít nhất 30 phút.",
                null);
                }
            }

           
            var vietnamNow = DateTimeHelper.ToVietnamTime(DateTime.UtcNow);
            var weekStartVietnam = DateTimeHelper.GetWeekStartDate(vietnamNow);
            var WeekEndVietnam = weekStartVietnam.AddDays(7);
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

          
            // Xác định loại đặt lịch
            if (request.StartTime >= WeekEndVietnam)
            {
                // --- Đặt tuần sau ---
                var nextWeekStartVN = WeekEndVietnam;
                var nextWeekEndVN = nextWeekStartVN.AddDays(7);
                if (request.EndTime > nextWeekEndVN)
                {
                    return (false,
                        "Bạn chỉ có thể đặt trong phạm vi 1 tuần. Thời gian kết thúc không được vượt qua tuần kế tiếp.",
                        null);
                }
                var nextWeekStartUtc = DateTimeHelper.ToUtcFromVietnamTime(nextWeekStartVN);
                var nextQuota = await _usageQuotaService.EnsureQuotaExistsAsync(userId, request.GroupId, request.VehicleId, nextWeekStartUtc);

                if (!nextQuota.IsSuccess)
                    return (false, nextQuota.Message, null);

                
                var bookingHours = (decimal)(endUtc - startUtc).TotalHours;

                if (quota.HoursUsed + quota.HoursAdvance + quota.HoursDebt + bookingHours > quota.HoursLimit)
                    return (false, "Bạn không đủ giờ để đặt trước cho tuần sau.", null);


                quota.HoursAdvance += bookingHours;
                quota.LastUpdated = DateTime.UtcNow;
                await _usageQuotaRepository.UpdateAsync(quota);
                await _usageQuotaRepository.SaveChangesAsync();
            }

            else if (request.StartTime < WeekEndVietnam && request.EndTime > WeekEndVietnam)
            {
                // --- Đặt lịch qua tuần ---
               decimal hoursThisWeek = (decimal)(WeekEndVietnam - request.StartTime).TotalHours;
                decimal hoursNextWeek = (decimal)(request.EndTime - WeekEndVietnam).TotalHours;

                if (quota.HoursUsed + hoursThisWeek > quota.HoursLimit)
                    return (false, "Giờ trong tuần này không đủ cho phần đặt lịch hiện tại.", null);

                quota.HoursUsed += hoursThisWeek;
                quota.LastUpdated = DateTime.UtcNow;
                await _usageQuotaRepository.UpdateAsync(quota);

                // quota tuần sau
                var nextWeekStartVN = WeekEndVietnam;
                var nextWeekStartUtc = DateTimeHelper.ToUtcFromVietnamTime(nextWeekStartVN);
                await _usageQuotaService.EnsureQuotaExistsAsync(userId, request.GroupId, request.VehicleId, nextWeekStartUtc);

               

                if (quota.HoursAdvance + quota.HoursDebt + hoursNextWeek > quota.HoursLimit)
                    return (false, "Bạn không đủ giờ ứng trước cho phần đặt qua tuần sau.", null);

                quota.HoursAdvance += hoursNextWeek;
                quota.LastUpdated = DateTime.UtcNow;
                await _usageQuotaRepository.UpdateAsync(quota);
                await _usageQuotaRepository.SaveChangesAsync();
            }
            else
            {
                // --- Đặt lịch trong tuần này ---
                var bookingHours = (decimal)(endUtc - startUtc).TotalHours;
                if (quota.HoursUsed + bookingHours > quota.HoursLimit)
                    return (false, $"Thời gian đặt lịch tuần này không đủ. Giờ còn lại: {(quota.HoursLimit - quota.HoursUsed):F2}h.", null);

                quota.HoursUsed += bookingHours;
                quota.LastUpdated = DateTime.UtcNow;
                await _usageQuotaRepository.UpdateAsync(quota);
                await _usageQuotaRepository.SaveChangesAsync();
            }

         

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

            if (booking.Status != BookingStatus.Booked)
                return (false, "Huỷ lịch không thành công");

            var nowUtc = DateTime.UtcNow;
            var vietnamNow = DateTimeHelper.ToVietnamTime(nowUtc);

            var weekStartVN = DateTimeHelper.GetWeekStartDate(vietnamNow);
            var weekEndVN = weekStartVN.AddDays(7);
            var weekStartUtc = DateTime.SpecifyKind(  DateTimeHelper.ToUtcFromVietnamTime(weekStartVN), DateTimeKind.Utc
           );

       

            var startTimeVN = DateTimeHelper.ToVietnamTime(booking.StartTime);
            var endTimeVN = DateTimeHelper.ToVietnamTime(booking.EndTime);
            var minutesUntilStart = (booking.StartTime - nowUtc).TotalMinutes;

            decimal hoursThisWeek = 0;
            decimal hoursNextWeek = 0;

            if (endTimeVN <= weekEndVN)
            {
                // → Lịch trong tuần này
                hoursThisWeek = (decimal)(booking.EndTime - booking.StartTime).TotalHours;
            }
            else if (startTimeVN >= weekEndVN)
            {
                // → Lịch tuần sau
                hoursNextWeek = (decimal)(booking.EndTime - booking.StartTime).TotalHours;
            }
            else
            {
                // → Lịch kéo dài qua tuần
                hoursThisWeek = (decimal)(weekEndVN - startTimeVN).TotalHours;
                hoursNextWeek = (decimal)(endTimeVN - weekEndVN).TotalHours;
            }


            var quota = await _usageQuotaRepository.GetUsageQuotaAsync(
                    booking.UserId, booking.GroupId, booking.VehicleId, weekStartUtc);

        

            if (quota == null )
                return (false, "Không thể lấy thông tin quota để hoàn giờ.");

            string message;

            if (minutesUntilStart >= 15)
            {
                if (hoursThisWeek > 0 && quota != null)
                {
                    quota.HoursUsed -= hoursThisWeek;
                    if (quota.HoursUsed < 0) quota.HoursUsed = 0;
                }

                if (hoursNextWeek > 0 && quota != null)
                {
                    quota.HoursAdvance -= hoursNextWeek;
                    if (quota.HoursAdvance < 0) quota.HoursAdvance = 0;
                }

                message = "Hủy lịch thành công. Giờ đã được hoàn lại.";
            }
            else
            {
                
                if (hoursThisWeek > 0 && quota != null)
                {
                    quota.HoursUsed -= hoursThisWeek;
                    if (quota.HoursUsed < 0) quota.HoursUsed = 0;
                    quota.HoursDebt += 0.5m;
                }

                if (hoursNextWeek > 0 && quota != null)
                {
                    quota.HoursAdvance -= hoursNextWeek;
                    if (quota.HoursAdvance < 0) quota.HoursAdvance = 0;
                    quota.HoursDebt += 0.5m;
                }

                message = "Hủy lịch trễ, bạn bị phạt 30 phút. Giờ đặt đã được hoàn lại.";
            }

            if (quota != null)
            {
                quota.LastUpdated = DateTime.UtcNow;
                await _usageQuotaRepository.UpdateAsync(quota);
            }
            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;
            await _bookingRepo.UpdateAsync(booking);

            await _usageQuotaRepository.SaveChangesAsync();
            await _bookingRepo.SaveChangesAsync();

            return (true, message);
        }

        public async Task<(bool IsSuccess, string Message)> CheckInAsync(TripEventRequestModel request, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return (false, "Không thể xác định người dùng.");

            var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
            if (booking == null)
                return (false, "Không tìm thấy lịch đặt.");

            if (booking.Status != BookingStatus.Booked)
                return (false, "Chỉ có thể check-in lịch đang ở trạng thái Booked.");

            var nowUtc = DateTime.UtcNow;
            var startUtc = booking.StartTime;
            var minutesUntilStart = (startUtc - nowUtc).TotalMinutes;

            //if (minutesUntilStart > 15)
            //{
            //    return (false, $"Bạn chỉ có thể check-in trong vòng 15 phút trước khi lịch bắt đầu. Hiện còn {Math.Floor(minutesUntilStart)} phút nữa.");
            //}
            //if (booking.StartTime.AddMinutes(15) >= nowUtc)
            //    return (false, "Checkin thất bại,bạn đã trễ quá 15p, vui lòng đến sớm hơn để checkin vào lần sau");

            String? photoUrl = await _firebaseStorageService.UploadFileAsync(request.Photo, "trip-events");
            if (request.Photo == null)
                return (false, "Checkin thất bại, vui lòng chụp ảnh và thử lại");
          
            booking.Status = BookingStatus.InUse;
            booking.UpdatedAt = DateTime.UtcNow;

            await _bookingRepo.UpdateAsync(booking);
            await _bookingRepo.SaveChangesAsync();


          
            var tripEvent = new TripEvent
            {
                Id = Guid.NewGuid(),
                EventType = "CHECKIN",
                SignedBy = Guid.Parse(userId),
                VehicleId = booking.VehicleId,
                BookingId = booking.Id,
                Description = "Nhân viên thực hiện check-in cho lịch đặt này.",
                PhotosUrl = photoUrl,
                CreatedAt = DateTime.UtcNow
            };
            await _tripEventRepository.AddAsync(tripEvent);
            return (true, "Check-in thành công.");
        }

        public async Task<(bool IsSuccess, string Message)> CheckOutAsync(TripEventRequestModel request, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return (false, "Không thể xác định người dùng.");

            var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
            if (booking == null)
                return (false, "Không tìm thấy lịch đặt.");

            if (booking.Status != BookingStatus.InUse && booking.Status != BookingStatus.Overtime)
                return (false, "Chỉ có thể check-out lịch đã check-in.");

            String? photoUrl = await _firebaseStorageService.UploadFileAsync(request.Photo, "trip-events");
            if (request.Photo == null)
                return (false, "Checkin thất bại, vui lòng chụp ảnh và thử lại");



            var vietnamNow = DateTimeHelper.ToVietnamTime(DateTime.UtcNow);
            var weekStartVN = DateTimeHelper.GetWeekStartDate(vietnamNow);
            var weekStartUtc = DateTime.SpecifyKind(DateTimeHelper.ToUtcFromVietnamTime(weekStartVN), DateTimeKind.Utc);
            var expectedEndVN = DateTimeHelper.ToVietnamTime(booking.EndTime);
            var overtimeMinutes = (vietnamNow - expectedEndVN).TotalMinutes;
            var quota = await _usageQuotaRepository.GetUsageQuotaAsync(
                booking.UserId, booking.GroupId, booking.VehicleId, weekStartUtc);

            if (quota == null)
                return (false, "Không thể lấy thông tin quota.");


            decimal plannedHours = (decimal)(booking.EndTime - booking.StartTime).TotalHours;

            if (overtimeMinutes <= -5)
            {
                var actualUsedHours = (decimal)(vietnamNow - DateTimeHelper.ToVietnamTime(booking.StartTime)).TotalHours;
                if (actualUsedHours < 0) actualUsedHours = 0;

                var diff = plannedHours - actualUsedHours;
                quota.HoursUsed -= diff;
                if (quota.HoursUsed < 0) quota.HoursUsed = 0;

                booking.EndTime = DateTimeHelper.ToUtcFromVietnamTime(vietnamNow);
                booking.Status = BookingStatus.Completed;
                booking.UpdatedAt = DateTime.UtcNow;

                await _usageQuotaRepository.UpdateAsync(quota);
                await _bookingRepo.UpdateAsync(booking);
                await _bookingRepo.SaveChangesAsync();

                return (true, $"Check-out sớm {Math.Abs(overtimeMinutes):F0} phút. Giờ sử dụng được điều chỉnh lại.");
            }

            else if (overtimeMinutes <= 5 && overtimeMinutes >= -5)
            {
                booking.Status = BookingStatus.Completed;
                booking.UpdatedAt = DateTime.UtcNow;

                await _bookingRepo.UpdateAsync(booking);
                await _bookingRepo.SaveChangesAsync();
                return (true, "Check-out thành công đúng giờ.");
            }
            else if (overtimeMinutes > 5 && overtimeMinutes < 15)
            {
                booking.Status = BookingStatus.Completed;
                booking.UpdatedAt = DateTime.UtcNow;

                await _bookingRepo.UpdateAsync(booking);
                await _bookingRepo.SaveChangesAsync();

                return (true, $"Check-out trễ {overtimeMinutes:F0} phút, vui lòng checkout sớm hơn vào lần sau.");
            }
            else if (overtimeMinutes >= 15 && overtimeMinutes <= 30)
            {
                quota.HoursDebt += 0.5m;
            }

            else
            {
                var overtimeHours = (decimal)(overtimeMinutes / 60);
                quota.HoursDebt += overtimeHours * 4;
            }

            if (quota.HoursDebt + plannedHours >= quota.HoursLimit)
            {
                quota.HoursDebt = quota.HoursLimit - plannedHours;
                if (quota.HoursDebt < 0) quota.HoursDebt = 0;
            }

            booking.Status = BookingStatus.Completed;
            booking.UpdatedAt = DateTime.UtcNow;

            await _usageQuotaRepository.UpdateAsync(quota);
            await _bookingRepo.UpdateAsync(booking);
            await _bookingRepo.SaveChangesAsync();


            var tripEvent = new TripEvent
            {
                Id = Guid.NewGuid(),
                EventType = "CHECKOUT",
                SignedBy = Guid.Parse(userId),
                BookingId = booking.Id,
                VehicleId = booking.VehicleId,
                Description = "Nhân viên thực hiện check-out cho lịch đặt này.",
                PhotosUrl = photoUrl,
                CreatedAt = DateTime.UtcNow
            };
            await _tripEventRepository.AddAsync(tripEvent);

            return (true, $"Check-out trễ {overtimeMinutes:F0} phút. Giờ phạt đã được cập nhật.");

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


        public async Task<IEnumerable<BookingResponseModel>> GetUserBookingHistoryByVehicleAsync(Guid userId, Guid vehicleId)
        {
            var bookings = await _bookingRepo.GetUserBookingsByVehicleAsync(userId, vehicleId);

            return bookings.Select(b => new BookingResponseModel
            {
                Id = b.Id,
                GroupId = b.GroupId,
                VehicleId = b.VehicleId,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            });
        }
        private Guid GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng trong token.");
            return Guid.Parse(userIdClaim);
        }

        public async Task<(bool IsSuccess, string Message, BookingResponseModel? Data)>
    UpdateBookingAsync(BookingUpdateRequestModel request)
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
                    if (b.Status == BookingStatus.Cancelled || b.Status == BookingStatus.Completed) continue;

                    var existingStartVN = DateTimeHelper.ToVietnamTime(b.StartTime.AddMinutes(-30));
                    var existingEndVN = DateTimeHelper.ToVietnamTime(b.EndTime.AddMinutes(30));

                    bool isOverlap =
                        request.StartTime < existingEndVN &&
                        request.EndTime > existingStartVN;

                    if (isOverlap)
                    {
                        return (false,
                    $"Xe này đã có người đặt từ {DateTimeHelper.ToVietnamTime(b.StartTime):HH:mm} đến {DateTimeHelper.ToVietnamTime(b.EndTime):HH:mm}. " +
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
                    var newDuration = (decimal)(newEndUtc - booking.StartTime).TotalHours;
                    var deltaHours = newDuration - oldDuration;

                    if (deltaHours > 0 && quota.HoursUsed + deltaHours > quota.HoursLimit)
                        return (false, $"Không đủ giờ sử dụng để kéo dài thời gian. Bạn còn {quota.HoursLimit - quota.HoursUsed:F2} giờ.", null);

                    quota.HoursUsed += deltaHours;
                    quota.LastUpdated = DateTime.UtcNow;
                    await _usageQuotaRepository.UpdateAsync(quota);
                    await _usageQuotaRepository.SaveChangesAsync();
                }

                booking.EndTime = newEndUtc;
                booking.UpdatedAt = DateTime.UtcNow;
                await _bookingRepo.UpdateAsync(booking);
                await _bookingRepo.SaveChangesAsync();

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
                    if (b.Status == BookingStatus.Cancelled || b.Status == BookingStatus.Completed) continue;

                    var existingStartVN = DateTimeHelper.ToVietnamTime(b.StartTime.AddMinutes(-30));
                    var existingEndVN = DateTimeHelper.ToVietnamTime(b.EndTime.AddMinutes(30));

                    bool isOverlap =
                        request.StartTime < existingEndVN &&
                        request.EndTime > existingStartVN;

                    if (isOverlap)
                    {
                        return (false,
                    $"Xe này đã có người đặt từ {DateTimeHelper.ToVietnamTime(b.StartTime):HH:mm} đến {DateTimeHelper.ToVietnamTime(b.EndTime):HH:mm}. " +
                    $"Vui lòng chọn thời gian cách ít nhất 30 phút.",
                    null);
                    }
                }

                var userId = booking.UserId;
                var groupId = booking.GroupId;
                var vehicleId = booking.VehicleId;

                var vietnamNow = DateTimeHelper.ToVietnamTime(DateTime.UtcNow);
                var weekStartVietnam = DateTimeHelper.GetWeekStartDate(vietnamNow);
                var WeekEndVietnam = weekStartVietnam.AddDays(7);
                var weekStartUtc = DateTime.SpecifyKind(
                    DateTimeHelper.ToUtcFromVietnamTime(weekStartVietnam),
                    DateTimeKind.Utc
                );

                var quota = await _usageQuotaRepository.GetUsageQuotaAsync(userId, groupId, vehicleId, weekStartUtc);

                if (quota == null)
                    return (false, "Không thể lấy thông tin quota.", null);

                var oldDuration = (decimal)(booking.EndTime - booking.StartTime).TotalHours;
                var newDuration = (decimal)(newEndUtc - newStartUtc).TotalHours;
                var deltaHours = newDuration - oldDuration;

                if (booking.StartTime < DateTimeHelper.ToUtcFromVietnamTime(WeekEndVietnam) &&
      booking.EndTime <= DateTimeHelper.ToUtcFromVietnamTime(WeekEndVietnam))
                {
                    // lịch cũ trong tuần này → trừ HoursUsed
                    quota.HoursUsed -= oldDuration;
                }
                else if (booking.StartTime >= DateTimeHelper.ToUtcFromVietnamTime(WeekEndVietnam))
                {
                    // lịch cũ tuần sau → trừ HoursAdvance
                    quota.HoursAdvance -= oldDuration;
                }
                else
                {
                    // lịch cũ qua tuần → chia đôi phần giờ
                    decimal oldHoursThisWeek = (decimal)(WeekEndVietnam - DateTimeHelper.ToVietnamTime(booking.StartTime)).TotalHours;
                    decimal oldHoursNextWeek = oldDuration - oldHoursThisWeek;
                    quota.HoursUsed -= oldHoursThisWeek;
                    quota.HoursAdvance -= oldHoursNextWeek;
                }

                if (request.StartTime >= WeekEndVietnam)
                {
                    // --- Cập nhật sang tuần sau ---
                    var nextWeekEndVN = WeekEndVietnam.AddDays(7);
                    if (request.EndTime > nextWeekEndVN)
                        return (false, "Bạn chỉ có thể cập nhật trong phạm vi 1 tuần. Không được vượt qua tuần kế tiếp.", null);

                    var bookingHours = (decimal)(newEndUtc - newStartUtc).TotalHours;

                    if (quota.HoursUsed + quota.HoursAdvance + quota.HoursDebt + bookingHours > quota.HoursLimit)
                        return (false, "Không đủ quota để cập nhật lịch tuần sau.", null);

                    quota.HoursAdvance += bookingHours;
                }
                else if (request.StartTime < WeekEndVietnam && request.EndTime > WeekEndVietnam)
                {
                    // --- Cập nhật lịch qua tuần ---
                    decimal hoursThisWeek = (decimal)(WeekEndVietnam - request.StartTime).TotalHours;
                    decimal hoursNextWeek = (decimal)(request.EndTime - WeekEndVietnam).TotalHours;

                    if (quota.HoursUsed + hoursThisWeek > quota.HoursLimit)
                        return (false, "Giờ trong tuần này không đủ cho phần cập nhật hiện tại.", null);

                    if (quota.HoursUsed + quota.HoursAdvance + quota.HoursDebt + hoursNextWeek > quota.HoursLimit)
                        return (false, "Giờ ứng trước không đủ cho phần cập nhật qua tuần sau.", null);

                    quota.HoursUsed += hoursThisWeek;
                    quota.HoursAdvance += hoursNextWeek;
                }
                else
                {
                    // --- Cập nhật trong tuần này ---
                    var bookingHours = (decimal)(newEndUtc - newStartUtc).TotalHours;

                    if (quota.HoursUsed + bookingHours > quota.HoursLimit)
                        return (false, $"Không đủ quota. Giờ còn lại: {(quota.HoursLimit - quota.HoursUsed):F2}h.", null);

                    quota.HoursUsed += bookingHours;
                }

                quota.LastUpdated = DateTime.UtcNow;
                await _usageQuotaRepository.UpdateAsync(quota);
                await _usageQuotaRepository.SaveChangesAsync();


                booking.StartTime = newStartUtc;
                booking.EndTime = newEndUtc;
                booking.UpdatedAt = DateTime.UtcNow;
                await _bookingRepo.UpdateAsync(booking);
                await _bookingRepo.SaveChangesAsync();

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
                Status = booking.Status,
                CreatedAt = DateTimeHelper.ToVietnamTime(booking.CreatedAt),
                UpdatedAt = DateTimeHelper.ToVietnamTime(booking.UpdatedAt)
            };
        }
    }
}