using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace SWP391BackEnd.Controllers
{
    [Route("api/vehicles")]

    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

    
        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

       
        // Lấy toàn bộ danh sách xe
        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            return Ok(vehicles);
        }

     
        // Lấy thông tin chi tiết 1 xe
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleById(Guid id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
                return NotFound(new { message = "Vehicle not found." });

            return Ok(vehicle);
        }

        
        // Thêm xe mới
        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody] Vehicle vehicle)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdVehicle = await _vehicleService.CreateVehicleAsync(vehicle);
            return CreatedAtAction(nameof(GetVehicleById),
                new { id = createdVehicle.Id },
                createdVehicle);
        }


        // Cập nhật thông tin xe
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(Guid id, [FromBody] Vehicle vehicle)
        {
            if (id != vehicle.Id)
                return BadRequest(new { message = "ID mismatch." });

            try
            {
                var updatedVehicle = await _vehicleService.UpdateVehicleAsync(vehicle);
                return Ok(updatedVehicle);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Vehicle not found." });
            }
        }


        // Xoá xe theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(Guid id)
        {
            try
            {
                var deletedVehicle = await _vehicleService.DeleteVehicleAsync(id);
                return Ok(deletedVehicle);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Vehicle not found." });
            }
        }

        
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveVehicles()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            var active = vehicles.Where(v => v.Status == "ACTIVE");
            return Ok(active);
        }
    }
}
