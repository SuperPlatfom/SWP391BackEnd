using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository
{
    public class EContractMemberShareRepository : IEContractMemberShareRepository
    {
        private readonly AppDbContext _db;
        public EContractMemberShareRepository(AppDbContext db) => _db = db;

        public async Task AddRangeAsync(IEnumerable<EContractMemberShare> shares)
        {
            await _db.EContractMemberShares.AddRangeAsync(shares);
        }

        public async Task<List<EContractMemberShare>> GetByContractIdAsync(Guid contractId)
        {
            return await _db.EContractMemberShares
                .Include(s => s.User)
                .Where(s => s.ContractId == contractId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task DeleteByContractIdAsync(Guid contractId)
        {
            var old = await _db.EContractMemberShares
                .Where(s => s.ContractId == contractId)
                .ToListAsync();
            if (old.Count > 0)
            {
                _db.EContractMemberShares.RemoveRange(old);
            }
        }

    }
}
