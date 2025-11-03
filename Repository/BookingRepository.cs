using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.Group)
                .Include(b => b.User)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(Guid id)
        {
            return await _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.Group)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
   
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
    
        }

        public async Task DeleteAsync(Booking booking)
        {
            _context.Bookings.Remove(booking);
 
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Bookings.AnyAsync(b => b.Id == id);
        }
        public async Task<List<Booking>> GetBookingsByVehicleAsync(Guid vehicleId)
        {
            return await _context.Bookings
                .Where(b => b.VehicleId == vehicleId)
                .ToListAsync();
        }
        public async Task<List<Booking>> GetUserBookingsByVehicleAsync(Guid userId, Guid vehicleId)
        {
            return await _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.Group)
                .Include(b => b.User)
                .Where(b => b.UserId == userId && b.VehicleId == vehicleId)
                .OrderByDescending(b => b.CreatedAt) // mới nhất → cũ nhất
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByVehicleInGroupAsync(Guid groupId, Guid vehicleId)
        {
            return await _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.Group)
                .Include(b => b.User)
                .Where(b => b.GroupId == groupId && b.VehicleId == vehicleId)
                .OrderBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
