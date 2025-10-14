using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ICoOwnershipGroupRepository _groupRepository; // thêm dependency

        public VehicleService(IVehicleRepository vehicleRepository, ICoOwnershipGroupRepository groupRepository)
        {
            _vehicleRepository = vehicleRepository;
            _groupRepository = groupRepository;
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

        public async Task<VehicleResponseModel> CreateVehicleAsync(Vehicle vehicle)
        {
            if (string.IsNullOrWhiteSpace(vehicle.Make))
                throw new ArgumentException("Vehicle make cannot be empty.");

            if (vehicle.GroupId.HasValue)
            {
                var group = await _groupRepository.GetByIdAsync(vehicle.GroupId.Value);
                if (group == null)
                    throw new KeyNotFoundException("GroupId does not exist.");
            }

            vehicle.CreatedAt = DateTime.UtcNow;
            vehicle.UpdatedAt = DateTime.UtcNow;

            var created = await _vehicleRepository.AddAsync(vehicle);
            return MapToResponseModel(created);
        }

        public async Task<VehicleResponseModel> UpdateVehicleAsync(Vehicle vehicle)
        {
            var existing = await _vehicleRepository.GetByIdAsync(vehicle.Id);
            if (existing == null)
                throw new KeyNotFoundException("Vehicle not found.");

            if (vehicle.GroupId.HasValue)
            {
                var group = await _groupRepository.GetByIdAsync(vehicle.GroupId.Value);
                if (group == null)
                    throw new KeyNotFoundException("GroupId does not exist.");
            }

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

        private static VehicleResponseModel MapToResponseModel(Vehicle v)
        {
            return new VehicleResponseModel
            {
                Id = v.Id,
                PlateNumber = v.PlateNumber,
                Make = v.Make,
                Model = v.Model,
                ModelYear = v.ModelYear,
                Color = v.Color,
                Status = v.Status ?? "Unknown",
                BatteryCapacityKwh = v.BatteryCapacityKwh,
                TelematicsDeviceId = v.TelematicsDeviceId,
                RangeKm = v.RangeKm,
                GroupId = v.GroupId
            };
        }
    }
}