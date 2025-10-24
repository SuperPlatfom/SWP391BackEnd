using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace SWP391BackEnd.Controllers
{
    [ApiController]
    [Route("api/booking")]
    [ApiExplorerSettings(GroupName = "Booking Management")]
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _bookingService.CreateBookingAsync(request, HttpContext.User);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message, data = result.Data });
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            var result = await _bookingService.CancelBookingAsync(id);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpPut("check-in/{id}")]
        public async Task<IActionResult> CheckIn(Guid id)
        {
            var result = await _bookingService.CheckInAsync(id);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpPut("check-out/{id}")]
        public async Task<IActionResult> CheckOut(Guid id)
        {
            var result = await _bookingService.CheckOutAsync(id);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpGet("Get-Booking-by-group-and-vehicle/{groupId}/{vehicleId}")]
        public async Task<IActionResult> GetBookingsByGroupAndVehicle(Guid groupId, Guid vehicleId)
        {
            var result = await _bookingService.GetBookingsByGroupAndVehicleAsync(groupId, vehicleId);

            if (!result.IsSuccess)
                return NotFound(new { message = result.Message });

            return Ok(new { message = result.Message, data = result.Data });
        }
    }
}
