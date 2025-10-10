using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IIdentityCardRepository
    {
        Task CreateAsync(CitizenIdentityCard identityCard);
        Task<CitizenIdentityCard?> GetByIdNumberAsync(string idNumber);
        Task<CitizenIdentityCard?> GetByUserIdAsync(Guid userId);
    }
}
