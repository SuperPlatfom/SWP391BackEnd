

using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using BusinessObject.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Service.Interfaces
{
    public interface IVehicleRequestService
    {
        Task<IEnumerable<VehicleRequestResponseModel>> GetAllRequestsAsync();
        Task<IEnumerable<VehicleRequestResponseModel>> GetMyRequestsAsync(ClaimsPrincipal user);
        Task<VehicleRequestResponseModel> GetRequestDetailAsync(Guid id);
        Task<VehicleRequest> CreateVehicleRequestAsync(VehicleRequestModel request, ClaimsPrincipal user);
    }
}
