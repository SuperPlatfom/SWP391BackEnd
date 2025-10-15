using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;

namespace Service
{
    public class ContractTemplateService : IContractTemplateService
    {
        private readonly IContractTemplateRepository _repo;

        public ContractTemplateService(IContractTemplateRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ContractTemplateSummaryDto>> GetAllAsync()
        {
            var templates = await _repo.GetAllAsync();

            return templates.Select(t => new ContractTemplateSummaryDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Version = t.Version,
                IsActive = t.IsActive,
                MinCoOwners = t.MinCoOwners,
                MaxCoOwners = t.MaxCoOwners,
                CreatedAt = DateTimeHelper.ToVietnamTime(t.CreatedAt),
                UpdatedAt = DateTimeHelper.ToVietnamTime(t.UpdatedAt)
            }).OrderByDescending(t => t.CreatedAt).ToList();
        }

        public async Task<ContractTemplateDto> GetDetailAsync(Guid id)
        {
            var template = await _repo.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException("Template not found");

            var dto = new ContractTemplateDto(template);

            dto.Clauses.ForEach(c =>
            {
                c.CreatedAt = DateTimeHelper.ToVietnamTime(c.CreatedAt);
                c.UpdatedAt = DateTimeHelper.ToVietnamTime(c.UpdatedAt);
            });

            dto.Variables.ForEach(v =>
            {
                v.CreatedAt = DateTimeHelper.ToVietnamTime(v.CreatedAt);
                v.UpdatedAt = DateTimeHelper.ToVietnamTime(v.UpdatedAt);
            });

            return dto;
        }


        public async Task<ContractTemplateDto> CreateAsync(CreateTemplateDto dto)
        {
            var template = new ContractTemplate
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Version = dto.Version ?? "1.0",
                Content = dto.Content,
                MinCoOwners = dto.MinCoOwners,
                MaxCoOwners = dto.MaxCoOwners,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.CreateAsync(template);
            return new ContractTemplateDto(template);
        }

        public async Task UpdateContentAsync(Guid id, string content)
        {
            var template = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Template not found");
            template.Content = content;
            template.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(template);
        }

        public async Task<ContractTemplateDto> UpdateTemplateAsync(Guid id, UpdateTemplateDto dto)
        {
            var template = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Template not found");

            template.Name = dto.Name;
            template.Description = dto.Description;
            template.Version = dto.Version ?? template.Version;
            template.MinCoOwners = dto.MinCoOwners;
            template.MaxCoOwners = dto.MaxCoOwners;
            template.IsActive = dto.IsActive;
            template.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(template);
            return new ContractTemplateDto(template);
        }


        public async Task DeleteAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<ContractClauseDto>> GetClausesAsync(Guid templateId)
        {
            var template = await _repo.GetByIdAsync(templateId) ?? throw new KeyNotFoundException("Template not found");

            return template.Clauses.OrderBy(c => c.OrderIndex).Select(c => new ContractClauseDto
            {
                Id = c.Id,
                Title = c.Title,
                Body = c.Body,
                OrderIndex = c.OrderIndex,
                IsMandatory = c.IsMandatory,
                CreatedAt = DateTimeHelper.ToVietnamTime(c.CreatedAt),
                UpdatedAt = DateTimeHelper.ToVietnamTime(c.UpdatedAt)
            }).ToList();
        }

        public async Task AddClauseAsync(Guid templateId, CreateClauseDto dto)
        {
            var exists = await _repo.GetByIdAsync(templateId);
            if (exists == null)
                throw new KeyNotFoundException("Template not found");

            var clause = new ContractClause
            {
                Id = Guid.NewGuid(),
                TemplateId = templateId,
                Title = dto.Title,
                Body = dto.Body,
                OrderIndex = dto.OrderIndex,
                IsMandatory = dto.IsMandatory,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repo.AddClauseAsync(clause);
        }

        public async Task UpdateClauseAsync(Guid clauseId, UpdateClauseDto dto)
        {
            var clause = await _repo.GetClauseByIdAsync(clauseId) ?? throw new KeyNotFoundException("Clause not found");
            clause.Title = dto.Title;
            clause.Body = dto.Body;
            clause.OrderIndex = dto.OrderIndex;
            clause.IsMandatory = dto.IsMandatory;
            clause.UpdatedAt = DateTime.UtcNow; 
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteClauseAsync(Guid clauseId)
        {
            var clause = await _repo.GetClauseByIdAsync(clauseId) ?? throw new KeyNotFoundException("Clause not found");
            _repo.DeleteClause(clause);
            await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<ContractVariableDto>> GetVariablesAsync(Guid templateId)
        {
            var template = await _repo.GetByIdAsync(templateId) ?? throw new KeyNotFoundException("Template not found");

            return template.Variables.Select(v => new ContractVariableDto
            {
                Id = v.Id,
                VariableName = v.VariableName,
                DisplayLabel = v.DisplayLabel,
                InputType = v.InputType,
                IsRequired = v.IsRequired,
                DefaultValue = v.DefaultValue,
                CreatedAt = DateTimeHelper.ToVietnamTime(v.CreatedAt),
                UpdatedAt = DateTimeHelper.ToVietnamTime(v.UpdatedAt)
            }).ToList();
        }

        public async Task AddVariableAsync(Guid templateId, CreateVariableDto dto)
        {
            var exists = await _repo.GetByIdAsync(templateId);
            if (exists == null)
                throw new KeyNotFoundException("Template not found");

            var variable = new ContractVariable
            {
                Id = Guid.NewGuid(),
                TemplateId = templateId,
                VariableName = dto.VariableName,
                DisplayLabel = dto.DisplayLabel,
                InputType = dto.InputType,
                IsRequired = dto.IsRequired,
                DefaultValue = dto.DefaultValue,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow


            };

            await _repo.AddVariableAsync(variable);
        }

        public async Task UpdateVariableAsync(Guid variableId, UpdateVariableDto dto)
        {
            var variable = await _repo.GetVariableByIdAsync(variableId) ?? throw new KeyNotFoundException("Variable not found");

            variable.DisplayLabel = dto.DisplayLabel;
            variable.InputType = dto.InputType;
            variable.IsRequired = dto.IsRequired;
            variable.DefaultValue = dto.DefaultValue;
            variable.UpdatedAt = DateTime.UtcNow;

            await _repo.SaveChangesAsync();
        }

        public async Task DeleteVariableAsync(Guid variableId)
        {
            var variable = await _repo.GetVariableByIdAsync(variableId) ?? throw new KeyNotFoundException("Variable not found");
            _repo.DeleteVariable(variable);
            await _repo.SaveChangesAsync();
        }
    }
}
