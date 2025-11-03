using BusinessObject.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;

namespace SWP391BackEnd.Controllers
{
    [Route("api/[controller]")]
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
        [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> CreateRequest([FromForm]VehicleRequestModel model)
        {
            var result = await _vehicleRequestService.CreateVehicleRequestAsync(model, User);
            return Ok(new
            {
                message = "Yêu cầu đăng ký xe đã được gửi, vui lòng chờ admin duyệt.",
                data = result
            });

        }
        [HttpPost("upload-test")]
        public async Task<IActionResult> UploadTest(IFormFile file)
        {
            if (file == null) return BadRequest("Chưa chọn file nào.");

            var url = await _firebaseStorageService.UploadFileAsync(file, "test-folder");
            return Ok(new { FileUrl = url });
        }

    }
}
