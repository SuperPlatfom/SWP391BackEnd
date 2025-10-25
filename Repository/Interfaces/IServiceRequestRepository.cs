using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IServiceRequestRepository
    {
        Task<IEnumerable<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(Guid id);
        Task AddAsync(ServiceRequest entity);
        Task UpdateAsync(ServiceRequest entity);
        Task<IEnumerable<ServiceRequest>> GetByGroupMembersAsync(Guid currentUserId);
        Task SaveChangesAsync();
    }
}
