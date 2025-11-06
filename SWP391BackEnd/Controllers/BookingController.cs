using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using SWP391BackEnd.Helpers;
using System.Security.Claims;

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

            return Ok(new { message = result.Message});
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateBooking([FromBody] BookingUpdateRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _bookingService.UpdateBookingAsync(request);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }


        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            var result = await _bookingService.CancelBookingAsync(id, User);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpPost("check-in/staff")]
        [Authorize (Roles ="Staff")]

        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CheckIn([FromForm] TripEventRequestModel checkin)
        {
            try
            {
                var result = await _bookingService.CheckInAsync(checkin,User);

                if (!result.IsSuccess)
                    return BadRequest(new { message = result.Message });

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 500);
            }
        }

        [HttpPost("check-out/staff")]
        [Authorize (Roles ="Staff")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CheckOut([FromForm] TripEventRequestModel checkout)
        {
            var result = await _bookingService.CheckOutAsync(checkout, User);

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
