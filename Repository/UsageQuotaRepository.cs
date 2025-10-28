using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;


namespace Repository
{
    public class UsageQuotaRepository : IUsageQuotaRepository
    {
        private readonly AppDbContext _context;

        public UsageQuotaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UsageQuota?> GetUsageQuotaAsync(Guid accountId, Guid groupId, Guid vehicleId, DateTime weekStart)
        {
            return await _context.UsageQuotas.FirstOrDefaultAsync(u =>
                u.AccountId == accountId &&
                u.GroupId == groupId &&
                u.VehicleId == vehicleId &&
                u.WeekStartDate == weekStart);
        }

        public async Task<(decimal weeklyQuotaHours, decimal? ownershipRate)?> GetQuotaRateAsync(Guid accountId, Guid vehicleId)
        {
            var result = await (
                from share in _context.EContractMemberShares
                join contract in _context.EContracts on share.ContractId equals contract.Id
                join vehicle in _context.Vehicles on contract.VehicleId equals vehicle.Id
                where share.UserId == accountId && vehicle.Id == vehicleId
                select new
                {
                    vehicle.WeeklyQuotaHours,
                    share.OwnershipRate
                }
            ).FirstOrDefaultAsync();

            if (result == null)
                return null;

            return (result.WeeklyQuotaHours, result.OwnershipRate);
        }
        public async Task AddAsync(UsageQuota quota)
        {
            await _context.UsageQuotas.AddAsync(quota);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
