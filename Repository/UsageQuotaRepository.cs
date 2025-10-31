using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System.Linq;
using System.Linq.Expressions;


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

        public async Task<IEnumerable<UsageQuota>> GetAllAsync(Expression<Func<UsageQuota, bool>>? filter = null)
        {
            IQueryable<UsageQuota> query = _context.UsageQuotas;

            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Reset toàn bộ HoursUsed về 0
        /// </summary>
        public async Task ResetAllQuotaHoursUsedAsync()
        {
            var quotas = await _context.UsageQuotas.ToListAsync();

            foreach (var quota in quotas)
            {
                quota.HoursUsed = 0;
            }

            await _context.SaveChangesAsync();
        }
        public async Task AddAsync(UsageQuota quota)
        {
            await _context.UsageQuotas.AddAsync(quota);
        }

        public async Task UpdateAsync(UsageQuota quota)
        {
  
            _context.UsageQuotas.Update(quota);
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
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
