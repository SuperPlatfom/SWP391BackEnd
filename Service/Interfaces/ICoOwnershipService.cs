using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface ICoOwnershipService
    {
        Task<GroupResponseModel> CreateGroupAsync(CreateGroupRequest request, Guid userId);
        Task<GroupMember> InviteMemberAsync(InviteRequest request);
        Task<List<BasicGroupReponseModel>> GetGroupsByCurrentUserAsync(ClaimsPrincipal user);

        Task<GroupResponseModel> GetGroupByIdAsync(Guid groupId);
        Task<GroupResponseModel> UpdateGroupAsync(Guid groupId, string newName);
        Task<List<GroupResponseModel>> GetAllGroupsAsync();
        Task <(bool IsSuccess, string message)> AttachVehicleToGroupAsync(Guid userId, Guid groupId, Guid vehicleId);
        Task DetachVehicleFromGroupAsync(Guid userId, Guid groupId, Guid vehicleId);
        Task<List<VehicleResponseModel>> GetVehiclesByGroupIdAsync(Guid groupId);
        Task<bool> DeactivateVehicleAsync(Guid vehicleId, Guid userId);
        Task<bool> ActivateVehicleAsync(Guid vehicleId, Guid userId);
    }
}
