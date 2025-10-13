using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace SWP391BackEnd.Controllers
{
    [ApiExplorerSettings(GroupName = "Co-ownership Group Management")]
    [ApiController]
    [Route("api/[controller]")]
    public class GroupMemberController : Controller
    {
        private readonly iGroupMemberService _service;

        public GroupMemberController(iGroupMemberService service)
        {
            _service = service;
        }

        [HttpPost("invite")]
        public async Task<IActionResult> InviteMember([FromQuery] InviteRequest request)
        {
            var member = await _service.InviteMemberAsync(request);
            return Ok(new { message = "Đã gửi lời mời", member });
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptInvite([FromQuery] InviteRequest request)
        {
            var result = await _service.AcceptInviteAsync(request);
            return Ok(new { message = "Tham gia nhóm thành công", result });
        }

        [HttpGet("pending-invites/{userId}")]
        public async Task<IActionResult> GetPendingInvites(Guid userId)
        {
            var invites = await _service.GetPendingInvitesAsync(userId);
            return Ok(invites);
        }

        [HttpGet("get-all-members-in-group/{groupId}")]
        public async Task<IActionResult> GetAllMembersInGroup(Guid groupId)
        {
            var members = await _service.GetAllMembersInGroupAsync(groupId);

            if (members == null || !members.Any())
                return NotFound(new { message = "Không tìm thấy thành viên nào trong nhóm hoặc nhóm không tồn tại." });

            return Ok(members);
        }
        [HttpDelete("deleteMember/{groupId}/{memberId}")]
        public async Task<IActionResult> DeleteMember(Guid groupId, Guid memberId)
        {
            var result = await _service.DeleteMemberAsync(groupId, memberId);
            return Ok(new { message = result });
        }
    }
}
