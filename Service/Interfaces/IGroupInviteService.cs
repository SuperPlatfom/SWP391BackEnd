
using System.Security.Claims;
using BusinessObject.DTOs.ResponseModels;

namespace Service.Interfaces
{
    public interface IGroupInviteService
    {
        Task<string> CreateInviteCodeAsync(Guid userId, Guid groupId);
         Task JoinGroupByInviteCodeAsync(Guid userId, string inviteCode);
    }
}
