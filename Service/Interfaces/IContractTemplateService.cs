
using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;

namespace Service.Interfaces
{
    public interface IContractTemplateService
    {
        Task<IEnumerable<ContractTemplateDto>> GetAllAsync();
        Task<ContractTemplateDto> GetDetailAsync(Guid id);
        Task<ContractTemplateDto> CreateAsync(CreateTemplateDto dto);
        Task<bool> UpdateContentAsync(Guid id, string content);
        Task AddClauseAsync(Guid templateId, CreateClauseDto dto);
        Task AddVariableAsync(Guid templateId, CreateVariableDto dto);
        Task DeleteAsync(Guid id);
    }
}
