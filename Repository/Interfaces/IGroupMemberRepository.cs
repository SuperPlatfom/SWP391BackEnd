using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IGroupMemberRepository
    {
        Task AddGroupMemberAsync(GroupMember member);
        Task<GroupMember?> GetGroupMemberAsync(Guid groupId, Guid userId);
        Task<bool> GroupMemberExistsAsync(Guid groupId, Guid userId);
        Task UpdateGroupMemberAsync(GroupMember member);
        Task<List<GroupMember>> GetPendingInvitesAsync(Guid userId);

        Task<List<GroupMember>> GetAcceptedMembersInGroupAsync(Guid groupId);

        Task<GroupMember?> GetByUserAndGroupAsync(Guid userId, Guid groupId);
        Task<bool> DeleteMemberAsync(Guid groupId, Guid memberId);
        Task<bool> IsGroupEmptyAsync(Guid groupId);
        Task<bool> DeleteGroupAsync(Guid groupId);
        Task<List<GroupMember>> GetByGroupIdAsync(Guid groupId);
   
    }
}
