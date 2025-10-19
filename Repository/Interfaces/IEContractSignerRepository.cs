using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IEContractSignerRepository
    {
        Task AddRangeAsync(IEnumerable<EContractSigner> signers);
        Task UpdateAsync(EContractSigner signer);
        Task<List<Guid>> GetContractIdsByUserAsync(Guid userId);
        Task<List<EContractSigner>> GetByContractIdAsync(Guid contractId);
        Task DeleteByContractIdAsync(Guid contractId);
    }
}
