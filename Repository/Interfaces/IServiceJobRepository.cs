using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IServiceJobRepository
    {
        Task<ServiceJob?> GetByRequestIdAsync(Guid requestId);
        Task AddAsync(ServiceJob entity);
        Task<IEnumerable<ServiceJob>> GetAllAsync();
        Task<ServiceJob?> GetByIdAsync(Guid id);
        Task UpdateAsync(ServiceJob entity);

        Task SaveChangesAsync();
    }
}
