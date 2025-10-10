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
    public class IdentityCardRepository : IIdentityCardRepository
    {
        private readonly AppDbContext _context;

        public IdentityCardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(CitizenIdentityCard identityCard)
        {
            await _context.CitizenIdentityCards.AddAsync(identityCard);
            await _context.SaveChangesAsync();
        }

        public async Task<CitizenIdentityCard?> GetByIdNumberAsync(string idNumber)
        {
            return await _context.CitizenIdentityCards
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdNumber == idNumber);
        }

        public async Task<CitizenIdentityCard?> GetByUserIdAsync(Guid userId)
        {
            return await _context.CitizenIdentityCards
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
