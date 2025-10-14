using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Services
{
    public class ContractTemplateService : IContractTemplateService
    {
        private readonly IContractTemplateRepository _repo;
        public ContractTemplateService(IContractTemplateRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ContractTemplateDto>> GetAllAsync()
        {
            var templates = await _repo.GetAllAsync();
            return templates.Select(t => new ContractTemplateDto(t)).ToList();
        }

        public async Task<ContractTemplateDto> GetDetailAsync(Guid id)
        {
            var template = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Template not found");
            return new ContractTemplateDto(template);
        }

        public async Task<ContractTemplateDto> CreateAsync(CreateTemplateDto dto)
        {
            var template = new ContractTemplate
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Version = dto.Version,
                Content = dto.Content,
                MinCoOwners = dto.MinCoOwners,
                MaxCoOwners = dto.MaxCoOwners,
                CreatedAt = DateTime.UtcNow
            };
            await _repo.CreateAsync(template);
            return new ContractTemplateDto(template);
        }

        public async Task<bool> UpdateContentAsync(Guid id, string content)
        {
            var template = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Template not found");
            template.Content = content;
            template.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(template);
            return true;
        }

        public async Task AddClauseAsync(Guid templateId, CreateClauseDto dto)
        {
            var clause = new ContractClause
            {
                Id = Guid.NewGuid(),
                TemplateId = templateId,
                Title = dto.Title,
                Body = dto.Body,
                OrderIndex = dto.OrderIndex,
                IsMandatory = dto.IsMandatory
            };
            await _repo.AddClauseAsync(clause);
        }

        public async Task AddVariableAsync(Guid templateId, CreateVariableDto dto)
        {
            var variable = new ContractVariable
            {
                Id = Guid.NewGuid(),
                TemplateId = templateId,
                VariableName = dto.VariableName,
                DisplayLabel = dto.DisplayLabel,
                InputType = dto.InputType,
                IsRequired = dto.IsRequired,
                DefaultValue = dto.DefaultValue
            };
            await _repo.AddVariableAsync(variable);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
