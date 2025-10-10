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
    public class VehicleRepository : IVehicleRepository
    {
        private readonly AppDbContext _context;

        public VehicleRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Vehicle> AddAsync(Vehicle vehicle)
        {
            vehicle.CreatedAt = DateTime.UtcNow;
            vehicle.UpdatedAt = DateTime.UtcNow;

            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<Vehicle> DeleteAsync(Guid id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
            }
            return vehicle!;
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _context.Vehicles.ToListAsync();
        }

        public async Task<Vehicle?> GetByIdAsync(Guid id)
        {
            return await _context.Vehicles.FindAsync(id);
        }

        public async Task<Vehicle> UpdateAsync(Vehicle vehicle)
        {
            vehicle.UpdatedAt = DateTime.UtcNow;

            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }
    }
}
