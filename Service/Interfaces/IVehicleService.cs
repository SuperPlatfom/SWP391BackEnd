using BusinessObject.DTOs.ResponseModels;
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
        Task<IEnumerable<VehicleResponseModel>> GetAllVehiclesAsync();
        Task<VehicleResponseModel?> GetVehicleByIdAsync(Guid id);
        Task<VehicleResponseModel> CreateVehicleAsync(Vehicle vehicle);
        Task<VehicleResponseModel> UpdateVehicleAsync(Vehicle vehicle);
        Task<VehicleResponseModel> DeleteVehicleAsync(Guid id);
    }
}
