using BusinessObject.Models;
using BusinessObject.DTOs.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<IEnumerable<Account>> GetAllOfficerAsync();
        Task<Account?> GetByIdAsync(Guid id);
        Task<Account> AddAsync(Account account);
        Task<Account> UpdateAsync(Account account);
        Task<Account> DeleteAsync(Guid id);
        Task<Account> UpdateOfficerAsync(Account account);
        Task<List<Account>> GetByIdsAsync(IEnumerable<Guid> ids);
    }
}
