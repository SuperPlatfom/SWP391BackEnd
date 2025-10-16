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
    public class GroupMemberRepository : IGroupMemberRepository
    {
        private readonly AppDbContext _context;

        public GroupMemberRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddGroupMemberAsync(GroupMember member)
        {
            _context.GroupMembers.Add(member);
            await _context.SaveChangesAsync();
        }

        public async Task<GroupMember?> GetGroupMemberAsync(Guid groupId, Guid userId)
        {
            return await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);
        }

        public async Task<bool> GroupMemberExistsAsync(Guid groupId, Guid userId)
        {
            return await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId);
        }

        public async Task UpdateGroupMemberAsync(GroupMember member)
        {
            _context.GroupMembers.Update(member);
            await _context.SaveChangesAsync();
        }

        public async Task<List<GroupMember>> GetPendingInvitesAsync(Guid userId)
        {
            return await _context.GroupMembers
                .Where(gm => gm.UserId == userId && gm.InviteStatus == "PENDING")
                .Include(gm => gm.Group)
                .ToListAsync();
        }
        public async Task<List<GroupMember>> GetAcceptedMembersInGroupAsync(Guid groupId)
        {
            return await _context.GroupMembers
                .Where(gm => gm.GroupId == groupId && gm.InviteStatus == "ACCEPTED")
                .Include(gm => gm.UserAccount)   // để lấy thông tin user từ Account
                .Include(gm => gm.Group)  // để lấy tên group
                .ToListAsync();
        }


        // xoá thành viên khỏi nhóm
        public async Task<bool> DeleteMemberAsync(Guid groupId, Guid memberId)
        {
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == memberId);

            if (member == null)
                return false;

            _context.GroupMembers.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }

        // kiểm tra xem nhóm còn thành viên ko
        public async Task<bool> IsGroupEmptyAsync(Guid groupId)
        {
            return !await _context.GroupMembers.AnyAsync(gm => gm.GroupId == groupId);
        }

        // Xoá nhóm
        public async Task<bool> DeleteGroupAsync(Guid groupId)
        {
            var group = await _context.CoOwnershipGroups.FirstOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
                return false;

            _context.CoOwnershipGroups.Remove(group);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<GroupMember?> GetByUserAndGroupAsync(Guid userId, Guid groupId)
        {
            return await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.UserId == userId && gm.GroupId == groupId && gm.IsActive);
        }
    }
}
