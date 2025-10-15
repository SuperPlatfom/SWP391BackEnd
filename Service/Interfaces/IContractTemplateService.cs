
using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;

namespace Service.Interfaces
{
    public interface IContractTemplateService
    {
        Task<IEnumerable<ContractTemplateSummaryDto>> GetAllAsync();
        Task<ContractTemplateDto> GetDetailAsync(Guid id);
        Task<ContractTemplateDto> CreateAsync(CreateTemplateDto dto);
        Task UpdateContentAsync(Guid id, string content);
        Task<ContractTemplateDto> UpdateTemplateAsync(Guid id, UpdateTemplateDto dto);
        Task AddClauseAsync(Guid templateId, CreateClauseDto dto);
        Task AddVariableAsync(Guid templateId, CreateVariableDto dto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<ContractClauseDto>> GetClausesAsync(Guid templateId);
        Task UpdateClauseAsync(Guid clauseId, UpdateClauseDto dto);
        Task DeleteClauseAsync(Guid clauseId);

        Task<IEnumerable<ContractVariableDto>> GetVariablesAsync(Guid templateId);
        Task UpdateVariableAsync(Guid variableId, UpdateVariableDto dto);
        Task DeleteVariableAsync(Guid variableId);
    }
}
