using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using BusinessObject.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleResponseModel>> GetAllVehiclesAsync();
        Task<VehicleResponseModel?> GetVehicleByIdAsync(Guid id);
    
        Task<(bool isSuccess, string Message)> DeleteVehicleAsync(Guid id);
        Task<List<VehicleOfUserResponseModel>> GetVehiclesByCreatorAsync(ClaimsPrincipal user);

    }
}
