using BusinessObject.Models;

namespace Repository.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string roleName);
    Task<IEnumerable<Role>> GetAllAsync();
    Task CreateAsync(Role role);
}