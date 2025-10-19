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
        Task<List<Guid>> GetContractIdsByUserAsync(Guid userId);
    }
}
