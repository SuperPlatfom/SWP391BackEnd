using BusinessObject.Models;
using BusinessObject.RequestModels;
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

        [HttpPost("create-vehicle")]
        public async Task<IActionResult> Create([FromQuery] VehicleRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                PlateNumber = request.PlateNumber,
                Make = request.Make,
                Model = request.Model,
                ModelYear = request.ModelYear,
                Color = request.Color,
                BatteryCapacityKwh = request.BatteryCapacityKwh,
                RangeKm = request.RangeKm,
                TelematicsDeviceId = request.TelematicsDeviceId,
                Status = request.Status,
                GroupId = request.GroupId, // ✅ thêm dòng này
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _vehicleService.CreateVehicleAsync(vehicle);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("update-vehicle-by-id")]
        public async Task<IActionResult> Update(Guid id, [FromQuery] VehicleRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _vehicleService.GetVehicleByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Vehicle not found" });

            // ✅ Tạo entity Vehicle mới để update
            var vehicleToUpdate = new Vehicle
            {
                Id = id,
                PlateNumber = request.PlateNumber,
                Make = request.Make,
                Model = request.Model,
                ModelYear = request.ModelYear,
                Color = request.Color,
                BatteryCapacityKwh = request.BatteryCapacityKwh,
                RangeKm = request.RangeKm,
                TelematicsDeviceId = request.TelematicsDeviceId,
                Status = request.Status,
                GroupId = request.GroupId,
                UpdatedAt = DateTime.UtcNow
            };

            var updated = await _vehicleService.UpdateVehicleAsync(vehicleToUpdate);
            return Ok(updated);
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
    }
}
