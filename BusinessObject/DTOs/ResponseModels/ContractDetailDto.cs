using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class ContractDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Status { get; set; } = "";
        public string? Content { get; set; }
        public string? FileUrl { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid TemplateId { get; set; }
        public string TemplateName { get; set; } = "";
        public string TemplateVersion { get; set; } = "";

        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = "";

        public Guid VehicleId { get; set; }
        public string? PlateNumber { get; set; }
        public string? VehicleName { get; set; } 
        public int ModelYear { get; set; }
        public string? Color { get; set; }
        public decimal BatteryCapacityKwh { get; set; }
        public int RangeKm { get; set; }


        public List<ContractSignerDto> Signers { get; set; } = new();
        public List<OwnershipShareDto> OwnershipShares { get; set; } = new();


        public AuditDto Audit { get; set; } = new();


        public PermissionDto Permissions { get; set; } = new();

    }
    public class OwnershipShareDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = "";
        public decimal Rate { get; set; }
    }

    public class AuditDto
    {
        public Guid CreatedBy { get; set; }
        public string CreatedByName { get; set; } = "";
        public DateTime? SignedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public Guid? ReviewedBy { get; set; }
        public string? ReviewNote { get; set; }
    }

    public class PermissionDto
    {
        public bool CanSign { get; set; }
        public bool CanSendOtp { get; set; }
        public bool CanReview { get; set; }
        public bool CanDownload { get; set; }
        public bool CanEditShares { get; set; }
    }

    public class ClauseDto 
    { 
        public string Title { get; set; } = ""; 
        public string Body { get; set; } = ""; 
        public int OrderIndex { get; set; } 
    }
    public class VariableDto 
    { 
        public string VariableName { get; set; } = ""; 
        public string DisplayLabel { get; set; } = ""; 
        public string InputType { get; set; } = ""; 
        public string? DefaultValue { get; set; } 
    }
}
