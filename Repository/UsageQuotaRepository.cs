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
