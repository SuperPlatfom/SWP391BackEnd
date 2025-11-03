using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Threading.Tasks;

namespace Repository
{
    public class ServiceJobRepository : IServiceJobRepository
    {
        private readonly AppDbContext _db;
        public ServiceJobRepository(AppDbContext db) => _db = db;

        public async Task<ServiceJob?> GetByRequestIdAsync(Guid requestId)
        {
            return await _db.ServiceJobs.FirstOrDefaultAsync(j => j.RequestId == requestId);
        }

        public async Task AddAsync(ServiceJob entity)
        {
            await _db.ServiceJobs.AddAsync(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
        public async Task<IEnumerable<ServiceJob>> GetAllAsync()
        {
            return await _db.ServiceJobs
                .Include(j => j.Request)
                .Include(j => j.Technician)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ServiceJob?> GetByIdAsync(Guid id)
        {
            return await _db.ServiceJobs
                .Include(j => j.Request)
                    .ThenInclude(r => r.Vehicle)
                .Include(j => j.Request)
                    .ThenInclude(r => r.Group)
                .Include(j => j.Request)
                    .ThenInclude(r => r.CreatedByAccount)
                .Include(j => j.Technician)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task UpdateAsync(ServiceJob entity)
        {
            _db.ServiceJobs.Update(entity);
            await Task.CompletedTask;
        }

    }
}
