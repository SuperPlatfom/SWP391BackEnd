using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IUsageQuotaService
    {
        Task<(bool IsSuccess, string Message, object? Data)> GetRemainingQuotaAsync(Guid groupId, Guid vehicleId, ClaimsPrincipal user);

    }
}
