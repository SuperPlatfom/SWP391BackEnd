
using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Service.Interfaces
{
    public interface IBookingService
    {
        Task<(bool IsSuccess, string Message)> CreateBookingAsync(BookingRequestModel request, ClaimsPrincipal user);
        Task<(bool IsSuccess, string Message)>
    UpdateBookingAsync(BookingUpdateRequestModel request, ClaimsPrincipal user);
        Task<(bool IsSuccess, string Message)> CancelBookingAsync(Guid bookingId, ClaimsPrincipal user);
        Task<(bool IsSuccess, string Message)> CancelBookingBackgroundServiceAsync(Guid bookingId);
        Task<(bool IsSuccess, string Message)> CheckInAsync(TripEventRequestModel tripEvent, ClaimsPrincipal user);
        Task<(bool IsSuccess, string Message)> CheckOutAsync(TripEventRequestModel tripEvent, ClaimsPrincipal user);
        Task<(bool IsSuccess, string Message, List<BookingResponseModel>? Data)>
        GetBookingsByGroupAndVehicleAsync(Guid groupId, Guid vehicleId);
        Task<IEnumerable<BookingResponseModel>> GetUserBookingHistoryByVehicleAsync(Guid userId, Guid vehicleId);

    }
}
