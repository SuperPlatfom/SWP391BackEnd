using BusinessObject.Models;

namespace Repository.Interfaces
{
    public interface IGroupInviteRepository
    {
        Task<GroupInvite> AddAsync(GroupInvite invite);
        Task<GroupInvite?> GetByCodeAsync(string code);
        Task UpdateAsync(GroupInvite invite);
    }
}