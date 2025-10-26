
using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using System.Security.Claims;

namespace Service.Interfaces
{
    public interface IBookingService
    {
        Task<(bool IsSuccess, string Message, BookingResponseModel? Data)> CreateBookingAsync(BookingRequestModel request, ClaimsPrincipal user);
        Task<(bool IsSuccess, string Message, BookingResponseModel? Data)>
    UpdateBookingAsync(BookingUpdateRequestModel request, ClaimsPrincipal user);
        Task<(bool IsSuccess, string Message)> CancelBookingAsync(Guid bookingId);
        Task<(bool IsSuccess, string Message)> CheckInAsync(Guid bookingId);
        Task<(bool IsSuccess, string Message)> CheckOutAsync(Guid bookingId);
        Task<(bool IsSuccess, string Message, List<BookingResponseModel>? Data)>
        GetBookingsByGroupAndVehicleAsync(Guid groupId, Guid vehicleId);

    }
}
