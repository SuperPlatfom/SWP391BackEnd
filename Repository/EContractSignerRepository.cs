using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EContractSignerRepository : IEContractSignerRepository
    {
        private readonly AppDbContext _db;
        public EContractSignerRepository(AppDbContext db) => _db = db;

        public async Task AddRangeAsync(IEnumerable<EContractSigner> signers)
        {
            await _db.EContractSigners.AddRangeAsync(signers);
        }
        public async Task<List<Guid>> GetContractIdsByUserAsync(Guid userId)
        {
            return await _db.EContractSigners
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .Select(s => s.ContractId)
                .Distinct()
                .ToListAsync();
        }
    }
}
