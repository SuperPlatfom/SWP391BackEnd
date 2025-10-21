using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("service_job")]
    public class ServiceJob
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("request_id")]
        public Guid RequestId { get; set; }

        [Column("technician_id")]
        public Guid TechnicianId { get; set; }

        [Column("scheduled_at")]
        public DateTime? ScheduledAt { get; set; }

        [Column("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [Column("report_url")]
        public string? ReportUrl { get; set; }

        [Column("status")]
        public string Status { get; set; } = "SCHEDULED";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ServiceRequest Request { get; set; } = null!;
        public Account Technician { get; set; } = null!;
    }
}
