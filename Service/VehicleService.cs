using BusinessObject.Models;
using Repository.Interfaces;
using Service.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            return await _vehicleRepository.GetAllAsync();
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(Guid id)
        {
            return await _vehicleRepository.GetByIdAsync(id);
        }

        public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
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

            return await _vehicleRepository.AddAsync(vehicle);
        }

        public async Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle)
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

            return await _vehicleRepository.UpdateAsync(vehicle);
        }

        public async Task<Vehicle> DeleteVehicleAsync(Guid id)
        {
            var existing = await _vehicleRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Vehicle not found.");

            return await _vehicleRepository.DeleteAsync(id);
        }
    }
}
