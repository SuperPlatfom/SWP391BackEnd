using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository;
using Repository.Interfaces;
using Service.Interfaces;
using System.Security.Claims;

namespace Service
{
    public class GroupInviteService : IGroupInviteService
    {
        private readonly IGroupInviteRepository _inviteRepository;
        private readonly ICoOwnershipGroupRepository _groupRepository;
        private readonly IGroupMemberRepository _memberRepository;

        public GroupInviteService(IGroupInviteRepository inviteRepository,
                                  ICoOwnershipGroupRepository groupRepository,
                                  IGroupMemberRepository memberRepository)
        {
            _inviteRepository = inviteRepository;
            _groupRepository = groupRepository;
            _memberRepository = memberRepository;
        }

        public async Task<string> CreateInviteCodeAsync(Guid userId, Guid groupId)
        {
            var membership = await _memberRepository.GetByUserAndGroupAsync(userId, groupId);
            if (membership == null)
                throw new UnauthorizedAccessException("Bạn không thuộc nhóm này.");

            if (membership.RoleInGroup != "OWNER")
                throw new UnauthorizedAccessException("Chỉ OWNER mới được tạo Invite Code.");

            var code = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

            var invite = new GroupInvite
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                InviteCode = code,
                ExpiresAt = DateTime.UtcNow.AddSeconds(30),
                CreatedAt = DateTime.UtcNow
            };

            await _inviteRepository.AddAsync(invite);
            return code;
        }

        public async Task JoinGroupByInviteCodeAsync(Guid userId, string inviteCode)
        {
            var invite = await _inviteRepository.GetByCodeAsync(inviteCode);
            if (invite == null)
                throw new KeyNotFoundException("Invite code không tồn tại.");

            if (invite.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invite code đã hết hạn.");

            var existing = await _memberRepository.GetByUserAndGroupAsync(userId, invite.GroupId);
            if (existing != null)
                throw new InvalidOperationException("Bạn đã là thành viên nhóm này.");

            var member = new GroupMember
            {
                Id = Guid.NewGuid(),
                GroupId = invite.GroupId,
                UserId = userId,
                RoleInGroup = "MEMBER",
                InviteStatus = "ACCEPTED",
                JoinDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _memberRepository.AddGroupMemberAsync(member);
        }
    }
}
