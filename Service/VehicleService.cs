using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using BusinessObject.RequestModels;
using Microsoft.AspNetCore.Http;
using Repository;
using Repository.Interfaces;
using Service.Interfaces;
using System.Security.Claims;

namespace Service
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IVehicleRequestRepository _vehicleRequestRepository;
    
        public VehicleService(IVehicleRepository vehicleRepository, IVehicleRequestRepository vehicleRequestRepository)
        {
            _vehicleRepository = vehicleRepository;
            _vehicleRequestRepository = vehicleRequestRepository;
        }

        public async Task<IEnumerable<VehicleResponseModel>> GetAllVehiclesAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();

            // ✅ Ánh xạ trực tiếp trong lệnh select
            var result = vehicles.Select(v => new VehicleResponseModel
            {
                Id = v.Id,
                PlateNumber = v.PlateNumber,
                Make = v.Make,
                Model = v.Model,
                ModelYear = v.ModelYear,
                Color = v.Color,
                Status = v.Status,
                BatteryCapacityKwh = v.BatteryCapacityKwh,
                RangeKm = v.RangeKm,
                GroupId = v.GroupId
            });

            return result;
        }

        public async Task<VehicleResponseModel?> GetVehicleByIdAsync(Guid id)
        {
            var v = await _vehicleRepository.GetByIdAsync(id);
            if (v == null)
                return null;

            return new VehicleResponseModel
            {
                Id = v.Id,
                PlateNumber = v.PlateNumber,
                Make = v.Make,
                Model = v.Model,
                ModelYear = v.ModelYear,
                Color = v.Color,
                Status = v.Status,
                BatteryCapacityKwh = v.BatteryCapacityKwh,
                RangeKm = v.RangeKm,
                GroupId = v.GroupId
            };
        }

        public async Task<VehicleRequestResponseModel> CreateVehicleAsync(VehicleRequestModel request, ClaimsPrincipal user)
        {
            if (user == null || !user.Identity?.IsAuthenticated == true)
                throw new UnauthorizedAccessException("Bạn cần đăng nhập để gửi yêu cầu tạo vehicle.");


            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Không thể xác định người dùng từ token.");

            var isPlateExist = await _vehicleRepository.ExistsAsync(v => v.PlateNumber == request.plateNumber);
            var isPendingPlate = await _vehicleRequestRepository.ExistsAsync(r => r.PlateNumber == request.plateNumber && r.Status == "PENDING");
            if (isPlateExist || isPendingPlate)
                throw new InvalidOperationException("Biển số này đã tồn tại hoặc đang chờ duyệt.");

        
            var vehicleRequest = new VehicleRequest
            {
                Id = Guid.NewGuid(),
                Type = "CREATE",
                PlateNumber = request.plateNumber,
                Make = request.make,
                Model = request.model,
                ModelYear = request.modelYear,
                Color = request.color,
                BatteryCapacityKwh = request.batteryCapacityKwh,
                RangeKm = request.rangeKm,
                Status = "PENDING",
                CreatedBy = Guid.Parse(userId),
                CreatedAt = DateTime.UtcNow,
                
            };

            var created = await _vehicleRequestRepository.AddAsync(vehicleRequest);

            return new VehicleRequestResponseModel
            {
                Id = created.Id,
                Type = created.Type,
                PlateNumber = created.PlateNumber,
                Make = created.Make,
                Model = created.Model,
                ModelYear = created.ModelYear,
                Color = created.Color,
                BatteryCapacityKwh = created.BatteryCapacityKwh,
                RangeKm = created.RangeKm,
                Status = created.Status,
                CreatedAt = created.CreatedAt
            };
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

    vehicle.Make = request.make;
    vehicle.Model = request.model;
    vehicle.ModelYear = request.modelYear;
    vehicle.Color = request.color;
    vehicle.BatteryCapacityKwh = request.batteryCapacityKwh;
    vehicle.RangeKm = request.rangeKm;
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


        public async Task<List<VehicleOfUserResponseModel>> GetVehiclesByCreatorAsync(ClaimsPrincipal user)
        {
            var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                throw new UnauthorizedAccessException("Không xác định được người dùng.");

            var userId = Guid.Parse(userIdStr);

            var vehicles = await _vehicleRepository.GetVehiclesByCreatorAsync(userId);
            return vehicles.Select(v => new VehicleOfUserResponseModel
            {
                Id = v.Id,
                Make = v.Make,
                Model = v.Model,
                ModelYear = v.ModelYear,
                Color = v.Color,
                PlateNumber = v.PlateNumber,
                Status = v.Status,
                BatteryCapacityKwh = v.BatteryCapacityKwh,
                RangeKm = v.RangeKm,
               HasGroup = v.GroupId !=null,
            }).ToList();
        }


    }
}