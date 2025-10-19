using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;
        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Account> AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<Account> DeleteAsync(Guid id)
        {
            Account? account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == id);
            if (account == null)
            {
                throw new InvalidOperationException("Account not found.");
            }
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _context.Accounts
                .Include(x => x.Role)
                .Include(x => x.CitizenIdentityCard)
                .ToListAsync();
        }

        public async Task<IEnumerable<Account>> GetAllOfficerAsync()
        {
            return await _context.Accounts
                .Include(x => x.Role)
                .Include(x => x.CitizenIdentityCard)
                .Where(x => x.Role.Name == "Officer")
                .ToListAsync();
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _context.Accounts
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Account> UpdateOfficerAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<Account> UpdateAsync(Account account)
        {
            return account;
        }

        
        public async Task<List<Account>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            if (ids == null) return new List<Account>();
            var list = ids.Distinct().ToList();
            if (list.Count == 0) return new List<Account>();

            return await _context.Accounts
                .AsNoTracking()
                .Where(a => list.Contains(a.Id))
                .Select(a => new Account
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    
                })
                .ToListAsync();
        }

    }
}
