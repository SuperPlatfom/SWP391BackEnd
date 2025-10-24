using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IUsageQuotaRepository
    {
        Task<UsageQuota?> GetUsageQuotaAsync(Guid accountId, Guid groupId, Guid vehicleId, DateTime weekStart);
        Task AddAsync(UsageQuota quota);
        Task SaveChangesAsync();
    }
}
