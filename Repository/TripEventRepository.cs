

using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository
{
    public class TripEventRepository : ITripEventRepository
    {
        private readonly AppDbContext _context;
        public TripEventRepository(AppDbContext context)
        {
            _context = context;
        }

       
        public async Task AddAsync(TripEvent tripEvent)
        {
           await _context.TripEvents.AddAsync(tripEvent);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TripEvent tripEvent)
        {
             _context.TripEvents.Remove(tripEvent);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TripEvent tripEvent)
        {
             _context.TripEvents.Update(tripEvent);
        }

     public async   Task<IEnumerable<TripEvent>> GetAllAsync()
        {
            return await _context.TripEvents.ToListAsync();
        }

        public async Task<IEnumerable<TripEvent>> GetDamageReportsByVehicleIdAsync(Guid vehicleId)
        {
            return await _context.TripEvents
                .Where(x => x.VehicleId == vehicleId && x.EventType == "DAMAGE")
                .Include(x => x.Vehicle)
                .Include(x => x.SignedByUser)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        public async Task<IEnumerable<TripEvent>> GetByUserIdAsync(Guid userId)
        {
            return await _context.TripEvents
                .Where(t => t.SignedBy == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
