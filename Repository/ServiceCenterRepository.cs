using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository
{
    public class ServiceCenterRepository : IServiceCenterRepository
    {
        private readonly AppDbContext _db;
        public ServiceCenterRepository(AppDbContext db) => _db = db;

        public async Task<IEnumerable<ServiceCenter>> GetAllAsync()
        {
            return await _db.ServiceCenters.AsNoTracking().ToListAsync();
        }

        public async Task<ServiceCenter?> GetByIdAsync(Guid id)
        {
            return await _db.ServiceCenters.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(ServiceCenter center)
        {
            await _db.ServiceCenters.AddAsync(center);
        }

        public async Task UpdateAsync(ServiceCenter center)
        {
            _db.ServiceCenters.Update(center);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _db.ServiceCenters.FindAsync(id);
            if (entity != null) _db.ServiceCenters.Remove(entity);
        }

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}
