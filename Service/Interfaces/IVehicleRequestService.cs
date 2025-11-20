

using BusinessObject.DTOs.RequestModels;
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
        Task<(bool IsSuccess, string Message)> CreateVehicleRequestAsync(VehicleRequestModel request, ClaimsPrincipal user);

        Task<(bool IsSuccess, string Message)> UpdateVehicleRequestAsync( VehicleUpdateModel model, ClaimsPrincipal user);
        Task<(bool IsSuccess, string Message)> ApproveRequestAsync(Guid requestId, ClaimsPrincipal user);
        Task<(bool IsSuccess, string Message)> RejectRequestAsync(Guid requestId, string reason, ClaimsPrincipal user);
        Task<(bool IsSuccess, string Message)> ApproveUpdateRequestAsync(Guid requestId, ClaimsPrincipal user);
        Task<(bool IsSuccess, string Message)> DeleteVehicleRequestAsync(Guid requestId, ClaimsPrincipal user);
    }
}
