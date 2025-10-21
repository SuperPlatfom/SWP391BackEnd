
using BusinessObject.Models;

namespace Service.Interfaces
{
    public interface IBookingService
    {
        Task<(bool IsSuccess, string Message, Booking? Data)> CreateBookingAsync(Booking booking);
        Task<(bool IsSuccess, string Message)> CancelBookingAsync(Guid bookingId);
        Task<(bool IsSuccess, string Message)> CheckInAsync(Guid bookingId);
        Task<(bool IsSuccess, string Message)> CheckOutAsync(Guid bookingId);

    }
}
