using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IEContractMemberShareRepository
    {
        Task AddRangeAsync(IEnumerable<EContractMemberShare> shares);
        Task<List<EContractMemberShare>> GetByContractIdAsync(Guid contractId);
        Task DeleteByContractIdAsync(Guid contractId);
        
    }
}
