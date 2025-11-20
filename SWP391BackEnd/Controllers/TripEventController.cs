using BusinessObject.DTOs;
using BusinessObject.DTOs.RequestModels;
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

        [HttpGet("All-trip-events/admin")]
        [Authorize(Roles = "Admin")]
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

        [HttpGet("Get-damage-report-by-vehicleId")]

        public async Task<IActionResult> GetDamageReportsByVehicleId([FromQuery] Guid vehicleId)
        {
            try
            {
                var reports = await _tripEventService.GetDamageReportsByVehicleId(vehicleId);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("History/staff")]
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

        [HttpPost("create-damage-report/staff")]
        [Authorize(Roles = "Staff")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateDamageReport([FromForm] TripDamageReportRequestModel request)
        {
            var result = await _tripEventService.ReportDamageAsync(request, User);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }
    }
}
