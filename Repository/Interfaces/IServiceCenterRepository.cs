using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IServiceCenterRepository
    {
        Task<IEnumerable<ServiceCenter>> GetAllAsync();
        Task<ServiceCenter?> GetByIdAsync(Guid id);
        Task AddAsync(ServiceCenter center);
        Task UpdateAsync(ServiceCenter center);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
