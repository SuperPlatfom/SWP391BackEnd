using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class ServiceJobDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TechnicianName { get; set; } = string.Empty;
        public DateTime? ScheduledAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ReportUrl { get; set; }
        public string Status { get; set; } = string.Empty;
        public string VehicleName { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string RequestCreatedBy { get; set; } = string.Empty;
        public string IssueDescription { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
    public class ServiceJobListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string TechnicianName { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ReportUrl { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
