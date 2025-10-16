using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.Security.Claims;

namespace SWP391BackEnd.Controllers
{
    [ApiExplorerSettings(GroupName = "Co-ownership Group Management")]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupInviteController : Controller
    {
        private readonly IGroupInviteService _groupService;

        public GroupInviteController(IGroupInviteService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost("{groupId}/create-invite")]
        public async Task<IActionResult> CreateInviteCode(Guid groupId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var code = await _groupService.CreateInviteCodeAsync(userId.Value, groupId);
                return Ok(new { InviteCode = code });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Tham gia nhóm bằng Invite Code
        [HttpPost("join-by-invite")]
        public async Task<IActionResult> JoinByInvite([FromQuery] string inviteCode)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            try
            {
                await _groupService.JoinGroupByInviteCodeAsync(userId.Value, inviteCode);
                return Ok(new { message = "Tham gia nhóm thành công" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Helper lấy userId từ token
        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return null;
            return Guid.Parse(userIdClaim);
        }
    }
}
