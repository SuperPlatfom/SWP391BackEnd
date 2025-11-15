

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("vehicle_request")]
    public class VehicleRequest
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("vehicle_id")]
        [ForeignKey(nameof(Vehicle))]
        public Guid? VehicleId { get; set; } 

        public Vehicle? Vehicle { get; set; }

        [Column("plate_number")]
        public string PlateNumber { get; set; } = null!;

        [Column("make")]
        public string Make { get; set; } = null!;

        [Column("model")]
        public string Model { get; set; } = null!;

        [Column("model_year")]
        public int ModelYear { get; set; } 

        [Column("color")]
        public string? Color { get; set; } = null!;

        [Column("battery_capacity_kwh")]
        public decimal? BatteryCapacityKwh { get; set; } 

        [Column("range_km")]
        public int? RangeKm { get; set; }

        [Column("vehicle_image_url")]
        public string VehicleImageUrl { get; set; } = null!;

        [Column("registration_paper_url")]
        public string RegistrationPaperUrl { get; set; } = null!;

        [Column("type")]
        public string Type { get; set; } = "CREATE";
       

        [Column("status")]
        public string Status { get; set; } = "PENDING";
       

        [Column("rejection_reason")]
        public string? RejectionReason { get; set; }

        [Column("created_by")]
        [ForeignKey(nameof(Requester))]
        public Guid CreatedBy { get; set; }
        public Account Requester { get; set; } = null!;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
