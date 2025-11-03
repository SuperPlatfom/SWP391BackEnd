using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Repository.Interfaces;
using Service.Interfaces;
using System.Security.Claims;

namespace Service
{
    public class TripEventService : ITripEventService
    {
        private readonly ITripEventRepository _tripEventRepository;

        public TripEventService(ITripEventRepository tripEventRepository)
        {
            _tripEventRepository = tripEventRepository;
        }
        public async Task<IEnumerable<TripEvent>> GetAllTripEvent()
        {
            return await _tripEventRepository.GetAllAsync();
        }

        public async Task<IEnumerable<TripEvent>> GetMyTripEvent(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Không thể xác định người dùng.");
            }
            return await _tripEventRepository.GetByUserIdAsync(Guid.Parse(userId));
        }


    }
}
