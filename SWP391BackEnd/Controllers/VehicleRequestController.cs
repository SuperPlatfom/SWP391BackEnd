using BusinessObject.DTOs.RequestModels;
using BusinessObject.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;

namespace SWP391BackEnd.Controllers
{
    [Route("api/vehicle-requests")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Vehicle Request Management")]
    [Authorize]
    public class VehicleRequestController : Controller
    {
        private readonly IVehicleRequestService _vehicleRequestService;
        private readonly IFirebaseStorageService _firebaseStorageService;

        public VehicleRequestController(IVehicleRequestService vehicleRequestService, IFirebaseStorageService firebaseStorageService)
        {
            _vehicleRequestService = vehicleRequestService;
            _firebaseStorageService = firebaseStorageService;
        }

       
        [HttpGet("my-requests")]
        [Authorize]
        public async Task<IActionResult> GetMyRequests()
        {
            var requests = await _vehicleRequestService.GetMyRequestsAsync(User);
            return Ok(requests);
        }

        
        [HttpGet("all")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests = await _vehicleRequestService.GetAllRequestsAsync();
            return Ok(requests);
        }

        
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetRequestDetail(Guid id)
        {
            var request = await _vehicleRequestService.GetRequestDetailAsync(id);
            return Ok(request);
        }

        [HttpPost("create")]
        [Consumes("multipart/form-data")]

        public async Task<IActionResult> CreateRequest([FromForm]VehicleRequestModel request)
        {
            var result = await _vehicleRequestService.CreateVehicleRequestAsync(request, User);
          if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            return Ok(new
            {
                message = result.Message,
            });

        }

        [HttpPost("update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateRequest([FromForm] VehicleUpdateModel request)
        {
            var result = await _vehicleRequestService.UpdateVehicleRequestAsync(request, User);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            return Ok(new
            {
                message = result.Message,
            });

        }


        [HttpPut("approve-create/{id}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var result = await _vehicleRequestService.ApproveRequestAsync(id, User);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpPost("approve-update/{id}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> ApproveUpdateRequest(Guid id)
        {
            try
            {
                var result = await _vehicleRequestService.ApproveUpdateRequestAsync(id, User);
                if (!result.IsSuccess)
                    return BadRequest(new { message = result.Message });

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("reject/{id}")]
        [Authorize(Roles = "Staff,Admin")]



        public async Task<IActionResult> Reject(Guid id, string reason)
        {
            var result = await _vehicleRequestService.RejectRequestAsync(id, reason, User);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteVehicleRequest(Guid id)
        {
            var result = await _vehicleRequestService.DeleteVehicleRequestAsync(id, User);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}
