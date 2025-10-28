using BusinessObject.Models;

namespace Repository.Interfaces
{
    public interface IUsageQuotaRepository
    {
        Task<UsageQuota?> GetUsageQuotaAsync(Guid accountId, Guid groupId, Guid vehicleId, DateTime weekStart);
        Task<(decimal weeklyQuotaHours, decimal? ownershipRate)?> GetQuotaRateAsync(Guid accountId, Guid vehicleId);
        Task AddAsync(UsageQuota quota);
        Task SaveChangesAsync();
    }
}
