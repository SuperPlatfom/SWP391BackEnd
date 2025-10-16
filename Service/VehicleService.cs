using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using BusinessObject.RequestModels;
using Microsoft.AspNetCore.Http;
using Repository.Interfaces;
using Service.Interfaces;
using System.Security.Claims;

namespace Service
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ICoOwnershipGroupRepository _groupRepository; // thêm dependency
        private readonly IHttpContextAccessor _httpContextAccessor;
        public VehicleService(
            IVehicleRepository vehicleRepository,
            ICoOwnershipGroupRepository groupRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _vehicleRepository = vehicleRepository;
            _groupRepository = groupRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<VehicleResponseModel>> GetAllVehiclesAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            return vehicles.Select(MapToResponseModel);
        }

        public async Task<VehicleResponseModel?> GetVehicleByIdAsync(Guid id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            return vehicle == null ? null : MapToResponseModel(vehicle);
        }

        public async Task<VehicleResponseModel> CreateVehicleAsync(VehicleRequestModel request, ClaimsPrincipal user)
        {
            if (user == null || !user.Identity?.IsAuthenticated == true)
                throw new UnauthorizedAccessException("Bạn cần đăng nhập để tạo vehicle.");

            // Lấy userId từ token JWT
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Không thể xác định người dùng từ token.");

            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                PlateNumber = request.PlateNumber,
                Make = request.Make,
                Model = request.Model,
                ModelYear = request.ModelYear,
                Color = request.Color,
                BatteryCapacityKwh = request.BatteryCapacityKwh,
                RangeKm = request.RangeKm,
                TelematicsDeviceId = request.TelematicsDeviceId,
                Status = "INACTIVE", // Mặc định ACTIVE
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = Guid.Parse(userId) // ✅ Gán người tạo
            };

            var created = await _vehicleRepository.AddAsync(vehicle);
            return MapToResponseModel(created);
        }

        private VehicleResponseModel MapToResponseModel(Vehicle vehicle)
        {
            return new VehicleResponseModel
            {
                Id = vehicle.Id,
                PlateNumber = vehicle.PlateNumber,
                Make = vehicle.Make,
                Model = vehicle.Model,
                ModelYear = vehicle.ModelYear,
                Color = vehicle.Color,
                Status = vehicle.Status,
                BatteryCapacityKwh = vehicle.BatteryCapacityKwh,
                TelematicsDeviceId = vehicle.TelematicsDeviceId,
                RangeKm = vehicle.RangeKm
            };
        }

        public async Task<VehicleResponseModel> UpdateVehicleAsync(Guid id, VehicleRequestModel request, ClaimsPrincipal user)
{
    var vehicle = await _vehicleRepository.GetByIdAsync(id);
    if (vehicle == null)
        throw new KeyNotFoundException("Vehicle not found");

    // xác thực người tạo (CreatedBy == userId)
    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (vehicle.CreatedBy.ToString() != userId)
        throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa vehicle này.");

    vehicle.Make = request.Make;
    vehicle.Model = request.Model;
    vehicle.ModelYear = request.ModelYear;
    vehicle.Color = request.Color;
    vehicle.BatteryCapacityKwh = request.BatteryCapacityKwh;
    vehicle.RangeKm = request.RangeKm;
    vehicle.TelematicsDeviceId = request.TelematicsDeviceId;
    vehicle.UpdatedAt = DateTime.UtcNow;

    var updated = await _vehicleRepository.UpdateAsync(vehicle);
    return MapToResponseModel(updated);
}

        public async Task<VehicleResponseModel> DeleteVehicleAsync(Guid id)
        {
            var existing = await _vehicleRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Vehicle not found.");

            var deleted = await _vehicleRepository.DeleteAsync(id);
            return MapToResponseModel(deleted);
        }


        public async Task<List<VehicleResponseModel>> GetVehiclesByCreatorAsync(ClaimsPrincipal user)
        {
            var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                throw new UnauthorizedAccessException("Không xác định được người dùng.");

            var userId = Guid.Parse(userIdStr);

            var vehicles = await _vehicleRepository.GetVehiclesByCreatorAsync(userId);
            return vehicles.Select(v => new VehicleResponseModel
            {
                Id = v.Id,
                Make = v.Make,
                Model = v.Model,
                ModelYear = v.ModelYear,
                Color = v.Color,
                PlateNumber = v.PlateNumber,
                BatteryCapacityKwh = v.BatteryCapacityKwh,
                RangeKm = v.RangeKm,
                TelematicsDeviceId = v.TelematicsDeviceId,
               
            }).ToList();
        }


    }
}