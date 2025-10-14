using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;


namespace SWP391BackEnd.Controllers
{
    [ApiExplorerSettings(GroupName = "Co-ownership Group Management")]
    [ApiController]
    [Route("api/[controller]")]
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
        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup([FromQuery] CreateGroupRequest request)
        {
            var group = await _service.CreateGroupAsync(request);
            return Ok(new { message = "Tạo nhóm thành công", group });
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

        [HttpPut("update-group")]
        public async Task<IActionResult> UpdateGroup(Guid groupId, [FromQuery] string newName, [FromQuery] string? newGovernancePolicy)
        {
            try
            {
                var updatedGroup = await _service.UpdateGroupAsync(groupId, newName, newGovernancePolicy);
                return Ok(new
                {
                    message = "Cập nhật nhóm thành công.",
                    updatedGroup
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpGet("user-groups/{userId}")]
        public async Task<IActionResult> GetGroupsByUser(Guid userId)
        {
            var groups = await _service.GetGroupsByUserAsync(userId);
            return Ok(groups);
        }

       
    }
}
