using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<Vehicle?> GetByIdAsync(Guid id);
        Task<Vehicle> AddAsync(Vehicle vehicle);
        Task<Vehicle> UpdateAsync(Vehicle vehicle);
        Task<Vehicle> DeleteAsync(Guid id);
        Task<List<Vehicle>> GetVehiclesByCreatorAsync(Guid creatorId);
        Task<bool> IsActiveInGroupAsync(Guid vehicleId, Guid groupId);
    }
}
