using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IServiceJobService
    {
        Task CreateAfterFullPaymentAsync(Guid expenseId);
        Task<IEnumerable<ServiceJobDto>> GetAllAsync(Guid? technicianId = null);
        Task<ServiceJobDto> GetByIdAsync(Guid id);
        Task UpdateStatusAsync(Guid jobId, UpdateServiceJobStatusRequest req, Guid technicianId);
        Task UpdateReportAsync(Guid jobId, UpdateServiceJobReportRequest req, Guid technicianId);
    }
}
