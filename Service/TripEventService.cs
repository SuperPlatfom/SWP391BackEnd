using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Google.Apis.Storage.v1;
using Microsoft.AspNetCore.Mvc.Filters;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;
using System.Security.Claims;

namespace Service
{
    public class TripEventService : ITripEventService
    {
        private readonly ITripEventRepository _tripEventRepository;
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly IBookingRepository _bookingRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly INotificationService _notificationService;
        public TripEventService(ITripEventRepository tripEventRepository, IFirebaseStorageService firebaseStorageService, IVehicleRepository vehicleRepository, IBookingRepository bookingRepository, IGroupMemberRepository groupMemberRepository, INotificationService notificationService)
        {
            _tripEventRepository = tripEventRepository;
            _firebaseStorageService = firebaseStorageService;
            _vehicleRepository = vehicleRepository;
            _bookingRepository = bookingRepository;
            _groupMemberRepository = groupMemberRepository;
            _notificationService = notificationService;
        }
        public async Task<IEnumerable<TripDamageReportResponse>> GetAllTripEvent()
        {
           var events = await _tripEventRepository.GetAllAsync();
            return events.Select(e => new TripDamageReportResponse
            {

                VehicleName = e.Vehicle.Model,
                VehiclePlate = e.Vehicle.PlateNumber,
                EventType = e.EventType,
                Description = e.Description,
                PhotosUrl = e.PhotosUrl,
                StaffName = e.SignedByUser.FullName,
                CreatedAt = DateTimeHelper.ToVietnamTime(e.CreatedAt),
            });
        }

        public async Task<IEnumerable<TripDamageReportResponse>> GetMyTripEvent(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Không thể xác định người dùng.");
            }
            var events = await _tripEventRepository.GetByUserIdAsync(Guid.Parse(userId));

            return events.Select(e => new TripDamageReportResponse
            {

                VehicleName = e.Vehicle.Model,
                VehiclePlate = e.Vehicle.PlateNumber,
                EventType = e.EventType,
                Description = e.Description,
                PhotosUrl = e.PhotosUrl,
                StaffName = e.SignedByUser.FullName,
                CreatedAt = DateTimeHelper.ToVietnamTime(e.CreatedAt),
            });
        }

       public async Task<IEnumerable<TripDamageReportResponse>> GetDamageReportsByVehicleId(Guid vehicleId)
        {
            var events = await _tripEventRepository.GetDamageReportsByVehicleIdAsync(vehicleId);
            return events.Select(e => new TripDamageReportResponse
            {

                VehicleName = e.Vehicle.Model,
                VehiclePlate = e.Vehicle.PlateNumber,
                Description = e.Description,
                PhotosUrl = e.PhotosUrl,
                StaffName = e.SignedByUser.FullName,
                CreatedAt = DateTimeHelper.ToVietnamTime(e.CreatedAt),
            });
        }

        public async Task<(bool IsSuccess, string Message)> ReportDamageAsync(TripDamageReportRequestModel request, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return (false, "Không thể xác định người dùng.");

           if (request.Photo == null)
                return (false, "Vui lòng tải lên hình ảnh bộ phận hư hỏng của xe.");

            List<string> imgUrls = new();
            if (request.Photo.Count > 4)
                return (false, "Chỉ được tải lên tối đa 4 hình ảnh.");
            foreach (var image in request.Photo)
            {
                var url = await _firebaseStorageService.UploadFileAsync(image, "Damage-report");
                imgUrls.Add(url);
            }

            if (request.Id == null)
                return (false, "Vui lòng nhập vào Id");

            var booking = await _bookingRepository.GetByIdAsync(request.Id);
            if (booking == null)
            {
                return (false, "Ko tìm thấy lịch xe");

            }

            var tripEvent = new TripEvent
            {
                Id = Guid.NewGuid(),
                EventType = "DAMAGE",
                SignedBy = Guid.Parse(userId),
                VehicleId = booking.VehicleId,
                BookingId = booking.Id,
                Description = request.Description ?? string.Empty,
                PhotosUrl = imgUrls.Any() ? System.Text.Json.JsonSerializer.Serialize(imgUrls) : null,
                CreatedAt = DateTime.UtcNow
            };

            var members2 = await _groupMemberRepository.GetByGroupIdAsync(booking.GroupId);
            foreach (var m in members2)
            {
                await _notificationService.CreateAsync(
                    m.UserId,
                    "Báo cáo hư hỏng",
                    $" Có báo cáo hư hỏng xe {booking.Vehicle.Model}",
                    "REPORT",
                    tripEvent.Id);
            }

            await _tripEventRepository.AddAsync(tripEvent);
            return (true, "Báo cáo thiệt hại đã được gửi thành công.");
        }

        
    }
}
