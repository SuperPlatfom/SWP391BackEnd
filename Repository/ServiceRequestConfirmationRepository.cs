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
    public class ServiceRequestConfirmationRepository : IServiceRequestConfirmationRepository
    {
        private readonly AppDbContext _db;
        public ServiceRequestConfirmationRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ServiceRequestConfirmation>> GetByRequestIdAsync(Guid requestId)
        {
            return await _db.ServiceRequestConfirmations
                .Where(c => c.RequestId == requestId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ServiceRequestConfirmation?> GetByUserAsync(Guid requestId, Guid userId)
        {
            return await _db.ServiceRequestConfirmations
                .FirstOrDefaultAsync(c => c.RequestId == requestId && c.UserId == userId);
        }

        public async Task AddAsync(ServiceRequestConfirmation entity)
        {
            await _db.ServiceRequestConfirmations.AddAsync(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
