using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System.Linq.Expressions;

namespace Repository.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByEmailAsync(string email)
    {
        return await _context.Accounts
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Email.ToLower().Equals(email.ToLower()));
    }

    public async Task<Account?> GetByPhoneAsync(string phone)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.Phone.Equals(phone));
    }

    public async Task<Account?> GetByIdNumberAsync(string idNumber)
    {
        return await _context.Accounts
            .Include(a => a.CitizenIdentityCard)
            .FirstOrDefaultAsync(a => a.CitizenIdentityCard.IdNumber.Equals(idNumber));
    }

    public async Task<Account> CreateAsync(Account account)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task UpdateAsync(Account account)
    {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }

    public async Task<Account> GetByRefreshTokenAsync(string refreshToken)
    {
        var user = await _context.Accounts.Include(a => a.Role)
            .Where(u => u.RefreshToken.Trim().ToLower() == refreshToken.Trim().ToLower())
            .FirstOrDefaultAsync();
        return user;
    }

    public List<Account> GetAll()
    {
        return _context.Accounts.ToList();
    }

    public Account GetAccountById(Guid id)
    {
        return _context.Accounts.Find(id);
    }

    public async Task<Account?> GetByIdAsync(Guid id)
    {
        return await _context.Accounts
            .Include(a => a.CitizenIdentityCard)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Account?> GetProfileByIdAsync(Guid id)
    {
        return await _context.Accounts
            .Include(a => a.CitizenIdentityCard)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Accounts.AnyAsync(a => a.Id == id);
    }

    public void AddAccount(Account account)
    {
        _context.Accounts.Add(account);
        _context.SaveChanges();
    }

    public async Task<bool> ExistsAsync(Expression<Func<Account, bool>> predicate)
    {
        return await _context.Accounts.AnyAsync(predicate);
    }
}