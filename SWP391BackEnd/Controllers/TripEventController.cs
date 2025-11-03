using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;

namespace SWP391BackEnd.Controllers
{
    [Route("api/trip-events")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Trip Event Management")]
    public class TripEventController : Controller
    {

        private readonly ITripEventService _tripEventService;

        public TripEventController(ITripEventService tripEventService)
        {
            _tripEventService = tripEventService;
        }

        [HttpGet("All-trip-events/staff")]
        public async Task<IActionResult> GetAllTripEvent()
        {
            try
            {
                var events = await _tripEventService.GetAllTripEvent();
                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("History")]
        public async Task<IActionResult> GetMyTripEvent()
        {
            try
            {
                var events = await _tripEventService.GetMyTripEvent(User);
                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
