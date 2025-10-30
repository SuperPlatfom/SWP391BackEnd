
using MediatR;
using System.Security.Claims;

namespace Service.Interfaces
{
    public interface IUsageQuotaService
    {
        Task<(bool IsSuccess, string Message, object? Data)> GetRemainingQuotaAsync(Guid groupId, Guid vehicleId, ClaimsPrincipal user);
        Task<(bool IsSuccess, string Message)> EnsureQuotaExistsAsync(Guid userId, Guid groupId, Guid vehicleId, DateTime weekStartUtc);
        Task ResetWeeklyQuotaAsync();
    }
}
