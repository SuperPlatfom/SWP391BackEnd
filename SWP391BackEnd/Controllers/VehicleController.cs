using BusinessObject.Models;
using BusinessObject.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace SWP391BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Vehicle Management")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet("get-all-vehicle")]
        public async Task<IActionResult> GetAll()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            return Ok(vehicles);
        }

        [HttpGet("get-vehicle-by-id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
                return NotFound(new { message = "Vehicle not found" });

            return Ok(vehicle);
        }

        [Authorize] // ✅ yêu cầu đăng nhập (có token)
        [HttpPost("create")]
        public async Task<IActionResult> CreateVehicle([FromBody] VehicleRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = HttpContext.User;
            var result = await _vehicleService.CreateVehicleAsync(request, user);

            return Ok(new
            {
                message = "Tạo vehicle thành công",
                data = result
            });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] VehicleRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = HttpContext.User;
            var result = await _vehicleService.UpdateVehicleAsync(id, request, user);
            return Ok(new { message = "Cập nhật vehicle thành công", data = result });
        }

        [HttpDelete("delete-vehicle-by-id")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _vehicleService.GetVehicleByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Vehicle not found" });

            var deleted = await _vehicleService.DeleteVehicleAsync(id);
            return Ok(deleted);
        }

        [Authorize]
        [HttpGet("my-vehicles")]
        public async Task<IActionResult> GetMyVehicles()
        {
            var user = HttpContext.User;
            var result = await _vehicleService.GetVehiclesByCreatorAsync(user);
            return Ok(result);
        }
    }
}
