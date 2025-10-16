using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;


namespace Repository
{
    public class GroupInviteRepository : IGroupInviteRepository
    {
        private readonly AppDbContext _context;
        public GroupInviteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GroupInvite> AddAsync(GroupInvite invite)
        {
            _context.GroupInvites.Add(invite);
            await _context.SaveChangesAsync();
            return invite;
        }

        public async Task<GroupInvite?> GetByCodeAsync(string code)
        {
            return await _context.GroupInvites
                .Include(i => i.Group)
                .FirstOrDefaultAsync(i => i.InviteCode == code);
        }

        public async Task UpdateAsync(GroupInvite invite)
        {
            _context.GroupInvites.Update(invite);
            await _context.SaveChangesAsync();
        }
    }
}
