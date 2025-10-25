using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IServiceRequestConfirmationRepository
    {
        Task<IEnumerable<ServiceRequestConfirmation>> GetByRequestIdAsync(Guid requestId);
        Task<ServiceRequestConfirmation?> GetByUserAsync(Guid requestId, Guid userId);
        Task AddAsync(ServiceRequestConfirmation entity);
        Task SaveChangesAsync();
    }
}
