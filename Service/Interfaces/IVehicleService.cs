using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle?> GetVehicleByIdAsync(Guid id);
        Task<Vehicle> CreateVehicleAsync(Vehicle vehicle);
        Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle);
        Task<Vehicle> DeleteVehicleAsync(Guid id);
    }
}
