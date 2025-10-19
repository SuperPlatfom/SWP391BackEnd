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
        public async Task UpdateAsync(EContractSigner signer)
        {
            _db.EContractSigners.Update(signer);
            await _db.SaveChangesAsync();
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
        public async Task<List<EContractSigner>> GetByContractIdAsync(Guid contractId)
        {
            return await _db.EContractSigners
                .Where(s => s.ContractId == contractId)
                .ToListAsync();
        }
        public async Task DeleteByContractIdAsync(Guid contractId)
        {

            await _db.EContractSigners.Where(s => s.ContractId == contractId).ExecuteDeleteAsync();

            var toRemove = await _db.EContractSigners.Where(s => s.ContractId == contractId).ToListAsync();
            if (toRemove.Count > 0) _db.EContractSigners.RemoveRange(toRemove);

        }
    }
}
