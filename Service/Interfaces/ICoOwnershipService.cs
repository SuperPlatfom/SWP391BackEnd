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
    public interface ICoOwnershipService
    {
        Task<GroupResponseModel> CreateGroupAsync(CreateGroupRequest request);
        Task<GroupMember> InviteMemberAsync(InviteRequest request);
        Task<List<GroupBasicReponseModel>> GetGroupsByUserAsync(Guid userId);

        Task<GroupResponseModel> GetGroupByIdAsync(Guid groupId);
        Task<GroupResponseModel> UpdateGroupAsync(Guid groupId, string newName, string? newGovernancePolicy);
        Task<List<GroupResponseModel>> GetAllGroupsAsync();
       
    }
}
