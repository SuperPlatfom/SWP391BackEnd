using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class ServiceRequestDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public string VehicleName { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public decimal? CostEstimate { get; set; }
        public DateTime? InspectionScheduledAt { get; set; }
        //public DateTime? EstimatedFinishAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? InspectionNotes { get; set; }
        public string? ServiceCenterName { get; set; }
        public string? ServiceCenterAddress { get; set; }
        public string? TechnicianName { get; set; }
        public string? ReportUrl { get; set; }
        public string? VehicleContractStatus { get; set; }
        public DateTime? ContractEffectiveFrom { get; set; }
        public DateTime? ContractExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
