using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IGroupExpenseRepository
    {
        Task<GroupExpense?> GetByIdAsync(Guid id);
        Task<IEnumerable<GroupExpense>> GetByGroupAsync(Guid groupId);
        Task<GroupExpense?> GetByRequestIdAsync(Guid requestId);
        Task AddAsync(GroupExpense entity);
        Task UpdateAsync(GroupExpense entity);
        Task SaveChangesAsync();
    }
}
