using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using BusinessObject.RequestModels;
using Microsoft.AspNetCore.Http;
using Repository.Interfaces;
using Service.Interfaces;
using System.Security.Claims;


namespace Service
{
    public class VehicleRequestService : IVehicleRequestService
    {
        private readonly IVehicleRequestRepository _vehicleRequestRepository;
        private readonly IFirebaseStorageService _storageService;
        public VehicleRequestService(IVehicleRequestRepository vehicleRequestRepository, IFirebaseStorageService storageService)
        {
            _vehicleRequestRepository = vehicleRequestRepository;
            _storageService = storageService;
        }

        public async Task<IEnumerable<VehicleRequestResponseModel>> GetAllRequestsAsync()
        {
            var requests = await _vehicleRequestRepository.GetAllRequestsAsync();
            return requests.Select(MapToResponseModel);
        }

        public async Task<IEnumerable<VehicleRequestResponseModel>> GetMyRequestsAsync(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Không thể xác định người dùng.");

            var requests = await _vehicleRequestRepository.GetRequestsByUserAsync(Guid.Parse(userId));
            return requests.Select(MapToResponseModel);
        }

        public async Task<VehicleRequestResponseModel> GetRequestDetailAsync(Guid id)
        {
            var request = await _vehicleRequestRepository.GetByIdWithDetailAsync(id);
            if (request == null)
                throw new KeyNotFoundException("Không tìm thấy yêu cầu này.");
            return MapToResponseModel(request);
        }

        public async Task<VehicleRequest> CreateVehicleRequestAsync(VehicleRequestModel request, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Không thể xác định người dùng.");

            if (!Guid.TryParse(userId, out var creatorId))
                throw new UnauthorizedAccessException("Token không hợp lệ.");

            if (request.vehicleImage == null || request.registrationPaper == null)
                throw new ArgumentException("Vui lòng upload đầy đủ ảnh xe và giấy tờ xe.");

            bool exists = await _vehicleRequestRepository.ExistsAsync(v => v.PlateNumber == request.plateNumber);
            if (exists)
                throw new InvalidOperationException("Biển số xe đã tồn tại trong hệ thống.");

            var vehicleImageUrl = await _storageService.UploadFileAsync(request.vehicleImage, "uploads");
            var registrationPaperUrl = await _storageService.UploadFileAsync(request.registrationPaper, "uploads");


            var vehicleRequest = new VehicleRequest
            {
                Id = Guid.NewGuid(),
                PlateNumber = request.plateNumber,
                Make = request.make,
                Model = request.model,
                ModelYear = request.modelYear,
                Color = request.color,
                BatteryCapacityKwh = request.batteryCapacityKwh,
                RangeKm = request.rangeKm,
                VehicleImageUrl = vehicleImageUrl,
                RegistrationPaperUrl = registrationPaperUrl,
                Type = "CREATE",
                Status = "PENDING",
                CreatedBy = creatorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _vehicleRequestRepository.AddAsync(vehicleRequest);
            return vehicleRequest;
        }

        private VehicleRequestResponseModel MapToResponseModel(VehicleRequest req)
        {
            return new VehicleRequestResponseModel
            {
                Id = req.Id,
                VehicleId = req.VehicleId,
                PlateNumber = req.PlateNumber,
                Make = req.Make,
                Model = req.Model,
                ModelYear = req.ModelYear,
                Color = req.Color,
                BatteryCapacityKwh = req.BatteryCapacityKwh,
                RangeKm = req.RangeKm,
                VehicleImageUrl = req.VehicleImageUrl,
                RegistrationPaperUrl = req.RegistrationPaperUrl,
                Type = req.Type,
                Status = req.Status,
                RejectionReason = req.RejectionReason,
                CreatedBy = req.CreatedBy,
                CreatedByName = req.Requester?.FullName,
                CreatedAt = req.CreatedAt,
                UpdatedAt = req.UpdatedAt
            };
        }
    }
}
