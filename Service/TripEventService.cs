using BusinessObject.DTOs.RequestModels;
using BusinessObject.Models;
using Google.Apis.Storage.v1;
using Microsoft.AspNetCore.Mvc.Filters;
using Repository.Interfaces;
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
        public TripEventService(ITripEventRepository tripEventRepository, IFirebaseStorageService firebaseStorageService, IVehicleRepository vehicleRepository, IBookingRepository bookingRepository)
        {
            _tripEventRepository = tripEventRepository;
            _firebaseStorageService = firebaseStorageService;
            _vehicleRepository = vehicleRepository;
            _bookingRepository = bookingRepository;
        }
        public async Task<IEnumerable<TripEvent>> GetAllTripEvent()
        {
            return await _tripEventRepository.GetAllAsync();
        }

        public async Task<IEnumerable<TripEvent>> GetMyTripEvent(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Không thể xác định người dùng.");
            }
            return await _tripEventRepository.GetByUserIdAsync(Guid.Parse(userId));
        }

        public async Task<(bool IsSuccess, string Message)> ReportDamageAsync(TripDamageReportRequestModel request, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return (false, "Không thể xác định người dùng.");

            string? photoUrl = null;
            if (request.Photo != null)
            {
                photoUrl = await _firebaseStorageService.UploadFileAsync(request.Photo, "trip-damage");
            }
            else
                return (false, "vui lòng chụp ảnh bộ phận bị hư hỏng");

            if (request.Id == null)
                return (false, "Vui lòng nhập vào Id");

            var booking = await _bookingRepository.GetByIdAsync(request.Id);
            if (booking == null)
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(request.Id);
                if (vehicle == null)
                    return (false, "Ko tìm thấy vehicleId hay bookingId truyền vào");

                var trip = new TripEvent
                {
                    Id = Guid.NewGuid(),
                    EventType = "DAMAGE",
                    SignedBy = Guid.Parse(userId),
                    VehicleId = vehicle.Id,
                    BookingId = null,
                    Description = request.Description ?? string.Empty,
                    PhotosUrl = photoUrl,
                    CreatedAt = DateTime.UtcNow
                };
                await _tripEventRepository.AddAsync(trip);
                return (true, "Báo cáo thiệt hại đã được gửi thành công.");

            }

            var tripEvent = new TripEvent
            {
                Id = Guid.NewGuid(),
                EventType = "DAMAGE",
                SignedBy = Guid.Parse(userId),
                VehicleId = booking.VehicleId,
                BookingId = booking.Id,
                Description = request.Description ?? string.Empty,
                PhotosUrl = photoUrl,
                CreatedAt = DateTime.UtcNow
            };


            await _tripEventRepository.AddAsync(tripEvent);
            return (true, "Báo cáo thiệt hại đã được gửi thành công.");
        }
    }
}
