using BusinessObject.Models;
using Repository;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Service
{
    using BusinessObject.Models;
    using Repository;
    using Repository.Interfaces;


    namespace Service.Implementations
    {
        public class VehicleService : IVehicleService
        {
            private readonly IVehicleRepository _vehicleRepository;

            // Inject repository qua constructor
            public VehicleService(IVehicleRepository vehicleRepository)
            {
                _vehicleRepository = vehicleRepository;
            }

            // Lấy danh sách tất cả vehicle
            public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
            {
                return await _vehicleRepository.GetAllAsync();
            }

            // Lấy 1 vehicle theo id
            public async Task<Vehicle?> GetVehicleByIdAsync(Guid id)
            {
                return await _vehicleRepository.GetByIdAsync(id);
            }

            // Thêm mới vehicle
            public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(vehicle.Make))
                    throw new ArgumentException("Vehicle make cannot be empty.");

                vehicle.CreatedAt = DateTime.UtcNow;
                vehicle.UpdatedAt = DateTime.UtcNow;

                return await _vehicleRepository.AddAsync(vehicle);
            }

            // Cập nhật vehicle
            public async Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle)
            {
                var existing = await _vehicleRepository.GetByIdAsync(vehicle.Id);
                if (existing == null)
                    throw new KeyNotFoundException("Vehicle not found.");

                vehicle.UpdatedAt = DateTime.UtcNow;

                return await _vehicleRepository.UpdateAsync(vehicle);
            }

            // Xoá vehicle
            public async Task<Vehicle> DeleteVehicleAsync(Guid id)
            {
                var existing = await _vehicleRepository.GetByIdAsync(id);
                if (existing == null)
                    throw new KeyNotFoundException("Vehicle not found.");

                return await _vehicleRepository.DeleteAsync(id);
            }
        }
    }

}
