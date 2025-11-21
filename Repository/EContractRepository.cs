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
    public class EContractRepository : IEContractRepository
    {
        private readonly AppDbContext _db;
        public EContractRepository(AppDbContext db) => _db = db;

        public async Task<EContract> AddAsync(EContract entity)
        {
            await _db.EContracts.AddAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(EContract contract)
        {
            _db.EContracts.Update(contract);
            await _db.SaveChangesAsync();
        }

        public async Task<EContract?> GetDetailAsync(Guid id)
        {
            return await _db.EContracts
                .Include(c => c.Template).ThenInclude(t => t.Clauses)
                .Include(c => c.Group)
                .Include(c => c.Vehicle)
                .Include(c => c.Signers).ThenInclude(s => s.User).ThenInclude(u => u.CitizenIdentityCard)
                .Include(c => c.MemberShares).ThenInclude(ms => ms.User)
                .Include(c => c.CreatedByAccount)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<EContract>> GetAllAsync()
        {
            return await _db.EContracts
                .Include(c => c.Template)
                .Include(c => c.Group)
                .Include(c => c.Vehicle)
                .ToListAsync();
        }

        public Task<IQueryable<EContract>> QueryAsync()
        {
            var q = _db.EContracts
                .Include(c => c.Template)
                .Include(c => c.Group)
                .Include(c => c.Vehicle)
                .AsQueryable();
            return Task.FromResult(q);
        }

        public async Task<EContract?> GetLatestApprovedByGroupAndVehicleAsync(Guid groupId, Guid vehicleId)
        {
            return await _db.EContracts
                .Where(c => c.GroupId == groupId
                            && c.VehicleId == vehicleId
                            && c.Status == "APPROVED"
                            && (c.EffectiveFrom == null || c.EffectiveFrom <= DateTime.UtcNow)
                            && (c.ExpiresAt == null || c.ExpiresAt >= DateTime.UtcNow))
                .OrderByDescending(c => c.EffectiveFrom) 
                .FirstOrDefaultAsync();
        }

        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
}
