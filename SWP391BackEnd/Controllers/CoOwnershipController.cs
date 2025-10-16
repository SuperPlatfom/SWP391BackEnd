using BusinessObject.DTOs.RequestModels;
using BusinessObject.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;
using System.Security.Claims;


namespace SWP391BackEnd.Controllers
{
    [ApiExplorerSettings(GroupName = "Co-ownership Group Management")]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CoOwnershipController : Controller
    {
        private readonly ICoOwnershipService _service;

        public CoOwnershipController(ICoOwnershipService service)
        {
            _service = service;
        }

        [HttpGet("all-groups")]
        public async Task<IActionResult> GetAllGroups()
        {
            var groups = await _service.GetAllGroupsAsync();
            return Ok(groups);
        }

        [HttpGet("my-groups")]
        public async Task<IActionResult> GetMyGroups()
        {
            try
            {
                var user = HttpContext.User; // Lấy user từ JWT/Authorize
                var groups = await _service.GetGroupsByCurrentUserAsync(user);
                return Ok(groups);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized("Không tìm thấy thông tin người dùng trong token.");

            var userId = Guid.Parse(userIdClaim);

            var group = await _service.CreateGroupAsync(request, userId);
            return Ok(new { message = "Tạo nhóm thành công", group });
        }

        [HttpPost("attach-vehicle")]
        [Authorize]
        public async Task<IActionResult> AttachVehicleToGroup([FromBody] AttachVehicleRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized("Không tìm thấy thông tin người dùng trong token.");

            var userId = Guid.Parse(userIdClaim);

            await _service.AttachVehicleToGroupAsync(userId, request.GroupId, request.VehicleId);
            return Ok(new { message = "Đã gán xe vào nhóm thành công." });
        }

        [HttpPost("detach-vehicle")]
        [Authorize]
        public async Task<IActionResult> DetachVehicleFromGroup([FromBody] AttachVehicleRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized("Không tìm thấy thông tin người dùng trong token.");

            var userId = Guid.Parse(userIdClaim);

            await _service.DetachVehicleFromGroupAsync(userId, request.GroupId, request.VehicleId);
            return Ok(new { message = "Đã gỡ xe ra khỏi nhóm thành công." });
        }

        [HttpGet("get-group-by-id")]
        public async Task<IActionResult> GetGroupById(Guid groupId)
        {
            try
            {
                var group = await _service.GetGroupByIdAsync(groupId);
                return Ok(group);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{groupId}/rename")]
        public async Task<IActionResult> UpdateGroupName(Guid groupId, [FromBody] CreateGroupRequest request)
        {
            var updatedGroup = await _service.UpdateGroupAsync(groupId, request.Name);
            return Ok(updatedGroup);
        }


        [HttpGet("{groupId}/vehicles")]
        public async Task<IActionResult> GetAllVehiclesInGroup(Guid groupId)
        {
            try
            {
                var vehicles = await _service.GetVehiclesByGroupIdAsync(groupId);
                return Ok(vehicles);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }



        }
        }
    }
