using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class CreateTemplateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Content { get; set; }
        public int MinCoOwners { get; set; } = 2;
        public int MaxCoOwners { get; set; } = 5;
    }

    public class CreateClauseDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public int OrderIndex { get; set; }
        public bool IsMandatory { get; set; } = true;
    }

    public class CreateVariableDto
    {
        public string VariableName { get; set; }
        public string DisplayLabel { get; set; }
        public string InputType { get; set; }
        public bool IsRequired { get; set; }
        public string? DefaultValue { get; set; }

    }

    public class UpdateContentDto
    {
        public string Content { get; set; }
    }
    public class UpdateTemplateDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? Version { get; set; }
        public int MinCoOwners { get; set; } = 2;
        public int MaxCoOwners { get; set; } = 5;
        public bool IsActive { get; set; } = true;
    }
    public class UpdateClauseDto
    {
        public string Title { get; set; } = default!;
        public string Body { get; set; } = default!;
        public int OrderIndex { get; set; }
        public bool IsMandatory { get; set; }
    }

    public class UpdateVariableDto
    {
        public string DisplayLabel { get; set; } = default!;
        public string InputType { get; set; } = "text";
        public bool IsRequired { get; set; }
        public string? DefaultValue { get; set; }
    }
}
