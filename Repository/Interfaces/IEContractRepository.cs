using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IEContractRepository
    {
        Task<EContract> AddAsync(EContract entity);
        Task UpdateAsync(EContract contract);
        Task<EContract?> GetDetailAsync(Guid id);
        Task<IQueryable<EContract>> QueryAsync(); 
        Task SaveChangesAsync();
    }
}
