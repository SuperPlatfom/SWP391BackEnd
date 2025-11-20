

using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using System.Security.Claims;

namespace Service.Interfaces
{
    public interface ITripEventService
    {
        Task<IEnumerable<TripDamageReportResponse>> GetAllTripEvent();
        Task<IEnumerable<TripDamageReportResponse>> GetMyTripEvent(ClaimsPrincipal user);
        Task<(bool IsSuccess, string Message)> ReportDamageAsync(TripDamageReportRequestModel request, ClaimsPrincipal user);
        Task<IEnumerable<TripDamageReportResponse>> GetDamageReportsByVehicleId(Guid vehicleId);
    }
}
