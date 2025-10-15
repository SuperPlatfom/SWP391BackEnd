using BusinessObject.Models;
using System.Text.Json.Serialization;

namespace BusinessObject.DTOs.ResponseModels
{
    public class ContractTemplateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Version { get; set; }
        public string Content { get; set; }
        public int MinCoOwners { get; set; }
        public int MaxCoOwners { get; set; }

        public List<ContractClauseDto> Clauses { get; set; } = new();
        public List<ContractVariableDto> Variables { get; set; } = new();

        public ContractTemplateDto(ContractTemplate t)
        {
            Id = t.Id;
            Name = t.Name;
            Description = t.Description;
            Version = t.Version;
            Content = t.Content;
            MinCoOwners = t.MinCoOwners;
            MaxCoOwners = t.MaxCoOwners;

            Clauses = t.Clauses
                .OrderBy(c => c.OrderIndex)
                .Select(c => new ContractClauseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Body = c.Body,
                    OrderIndex = c.OrderIndex,
                    IsMandatory = c.IsMandatory,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList();

            Variables = t.Variables.Select(v => new ContractVariableDto
            {
                Id = v.Id,
                VariableName = v.VariableName,
                DisplayLabel = v.DisplayLabel,
                InputType = v.InputType,
                IsRequired = v.IsRequired,
                DefaultValue = v.DefaultValue,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt
            }).ToList();
        }
    }

    public class ContractClauseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int OrderIndex { get; set; }
        public bool IsMandatory { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [JsonIgnore] 
        public ContractTemplateDto? Template { get; set; }
    }

    public class ContractVariableDto
    {
        public Guid Id { get; set; }
        public string VariableName { get; set; }
        public string DisplayLabel { get; set; }
        public string InputType { get; set; }
        public bool IsRequired { get; set; }
        public string? DefaultValue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [JsonIgnore] 
        public ContractTemplateDto? Template { get; set; }
    }

    public class ContractTemplateSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string Version { get; set; } = "1.0";
        public bool IsActive { get; set; }
        public int MinCoOwners { get; set; }
        public int MaxCoOwners { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }



}
