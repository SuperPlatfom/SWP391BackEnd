using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IContractService
    {
        Task<ContractDetailDto> CreateAsync(CreateContractRequest req, Guid currentUserId);
        Task<ContractDetailDto> UpdateAsync(Guid id, CreateContractRequest req, Guid currentUserId);
        Task<IEnumerable<ContractSummaryDto>> GetAllAsync(Guid currentUserId, string? status = null, Guid? groupId = null);
        Task<ContractDetailDto> GetDetailAsync(Guid id, Guid currentUserId);
        Task<IEnumerable<ContractSummaryDto>> GetMyContractsAsync(Guid currentUserId);
        Task<string> GetPreviewHtmlAsync(Guid id, Guid currentUserId);
        Task SendOtpAsync(Guid contractId, Guid signerUserId);
        Task VerifyOtpAsync(Guid contractId, Guid signerUserId, string otp);
        Task UpdateStatusAsync(Guid contractId, string newStatus);
        Task CancelContractAsync(Guid contractId, Guid currentUserId);
        Task ReviewContractAsync(Guid contractId, Guid staffId, bool approve, string? note);
    }
}
