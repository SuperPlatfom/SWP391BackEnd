using BusinessObject.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("service_request")]
    public class ServiceRequest
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("group_id")]
        public Guid GroupId { get; set; }

        [Column("vehicle_id")]
        public Guid VehicleId { get; set; }

        [Column("type")]
        public ServiceRequestType Type { get; set; }  // MAINTENANCE/REPAIR/CLEANING/UPGRADE/INSPECTION

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_by")]
        public Guid CreatedBy { get; set; }

        [Column("technician_id")]
        public Guid? TechnicianId { get; set; }

        [Column("cost_estimate", TypeName = "decimal(18,2)")]
        public decimal? CostEstimate { get; set; }

        [Column("inspection_scheduled_at")]
        public DateTime? InspectionScheduledAt { get; set; }

        [Column("service_center_id")]
        public Guid? ServiceCenterId { get; set; }

        [Column("inspection_notes")]
        public string? InspectionNotes { get; set; }

        [Column("status")]
        public string Status { get; set; } = "DRAFT";

        [Column("approved_at")]
        public DateTime? ApprovedAt { get; set; }

        [Column("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public CoOwnershipGroup Group { get; set; } = null!;
        public Vehicle Vehicle { get; set; } = null!;
        public Account CreatedByAccount { get; set; } = null!;
        public Account? Technician { get; set; }
        public GroupExpense? GroupExpense { get; set; }

        public ICollection<ServiceRequestConfirmation> Confirmations { get; set; } = new List<ServiceRequestConfirmation>();
        public ServiceJob? Job { get; set; }
        public ServiceCenter? ServiceCenter { get; set; }
    }
}
