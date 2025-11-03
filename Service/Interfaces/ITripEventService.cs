

using BusinessObject.Models;
using System.Security.Claims;

namespace Service.Interfaces
{
    public interface ITripEventService
    {
        Task<IEnumerable<TripEvent>> GetAllTripEvent();
        Task<IEnumerable<TripEvent>> GetMyTripEvent(ClaimsPrincipal user);
    }
}
