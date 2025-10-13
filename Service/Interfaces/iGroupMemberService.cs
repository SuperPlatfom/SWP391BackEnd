using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface iGroupMemberService
    {
        Task<GroupMember> InviteMemberAsync(InviteRequest request);
        Task<GroupMember> AcceptInviteAsync(InviteRequest request);
        Task<List<PendingInvitesResponseModel>> GetPendingInvitesAsync(Guid userId);
        Task<List<GroupMemberResponseModel>> GetAllMembersInGroupAsync(Guid groupId);
        Task<string> DeleteMemberAsync(Guid groupId, Guid memberId);
    }
}
