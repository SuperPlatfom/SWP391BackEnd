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
        Task<VehicleResponseModel> CreateVehicleAsync(VehicleRequestModel request, ClaimsPrincipal user);
        Task<VehicleResponseModel> UpdateVehicleAsync(Guid id, VehicleRequestModel request, ClaimsPrincipal user);
        Task<VehicleResponseModel> DeleteVehicleAsync(Guid id);
        Task<List<VehicleOfUserResponseModel>> GetVehiclesByCreatorAsync(ClaimsPrincipal user);

    }
}
