using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Service.Interfaces;
using BusinessObject.DTOs.ResponseModels;

namespace Service
{
    public class GroupMemberService : iGroupMemberService
    {
        private readonly  IGroupMemberRepository _repository;

        public GroupMemberService(IGroupMemberRepository repository)
        {
            _repository = repository;
        }
        public async Task<GroupMember> InviteMemberAsync(InviteRequest request)
        {
            var exists = await _repository.GroupMemberExistsAsync(request.GroupId, request.UserId);
            if (exists)
                throw new InvalidOperationException("User đã được mời hoặc đã tham gia nhóm.");

            var member = new GroupMember
            {
                Id = Guid.NewGuid(),
                GroupId = request.GroupId,
                UserId = request.UserId,
                RoleInGroup = "MEMBER",
                InviteStatus = "PENDING",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _repository.AddGroupMemberAsync(member);
            return member;
        }

        public async Task<GroupMember> AcceptInviteAsync(InviteRequest request)
        {
            var invite = await _repository.GetGroupMemberAsync(request.GroupId, request.UserId);
            if (invite == null)
                throw new KeyNotFoundException("Không tìm thấy lời mời.");

            invite.InviteStatus = "ACCEPTED";
            invite.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateGroupMemberAsync(invite);
            return invite;
        }

        public async Task<List<PendingInvitesResponseModel>> GetPendingInvitesAsync(Guid userId)
        {
            var invites = await _repository.GetPendingInvitesAsync(userId);

            return invites.Select(gm => new PendingInvitesResponseModel
            {
                GroupId = gm.GroupId,
                GroupName = gm.Group.Name,
                UserId = gm.UserId,
                RoleInGroup = gm.RoleInGroup,
                InviteStatus = gm.InviteStatus,
                JoinDate = gm.JoinDate
            }).ToList();
        }

        public async Task<List<GroupMemberResponseModel>> GetAllMembersInGroupAsync(Guid groupId)
        {
            var members = await _repository.GetAcceptedMembersInGroupAsync(groupId);

            // ✅ Map sang ResponseModel để tránh vòng lặp và trả thông tin rõ ràng
            return members.Select(m => new GroupMemberResponseModel
            {
                UserId = m.UserAccount.Id,
                FullName = m.UserAccount.FullName ?? "Unknown",  // tuỳ vào field trong Account (ví dụ: FullName, Username,…)
                GroupId = m.GroupId,
                GroupName = m.Group?.Name ?? "Unknown Group",
                RoleInGroup = m.RoleInGroup,
                InviteStatus = m.InviteStatus,
            }).ToList();
        }

        //  Xoá thành viên, tự động xoá nhóm nếu rỗng
        public async Task<string> DeleteMemberAsync(Guid groupId, Guid memberId)
        {
            var deleted = await _repository.DeleteMemberAsync(groupId, memberId);
            if (!deleted)
                return "Không tìm thấy thành viên trong nhóm.";

            bool isEmpty = await _repository.IsGroupEmptyAsync(groupId);
            if (isEmpty)
            {
                var groupDeleted = await _repository.DeleteGroupAsync(groupId);
                if (groupDeleted)
                    return "Đã xoá thành viên và nhóm (nhóm không còn ai).";
            }

            return "Đã xoá thành viên khỏi nhóm.";
        }

    }
}
