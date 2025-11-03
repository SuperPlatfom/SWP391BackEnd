using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BusinessObject.Models
{
    [Table("vehicle")]
    [Index(nameof(PlateNumber), IsUnique = true)]
    public class Vehicle
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("plate_number")]
        public string PlateNumber { get; set; } = null!;

        [Column("make")]
        public string Make { get; set; } = null!;

        [Column("model")]
        public string Model { get; set; } = null!;
        [Column("model_year")]
        public int ModelYear { get; set; }

        [Column("color")]
        public string Color { get; set; } = null!;   

        [Column("battery_capacity_kwh")]
        public decimal BatteryCapacityKwh { get; set; } 

        [Column("range_km")]
        public int RangeKm { get; set; }         
        [Column("status")]
        public string Status { get; set; } =   "INACTIVE";

        [Column("vehicle_image_url")]
        public string VehicleImageUrl { get; set; } = null!;

        [Column("registration_paper_url")]
        public string RegistrationPaperUrl { get; set; } = null!;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("weekly_quota_hours")]
        public decimal WeeklyQuotaHours { get; set; } = 112;
        [Column("created_by")]
        [ForeignKey(nameof(Creator))]
        public Guid CreatedBy { get; set; }

        public Account Creator { get; set; } = null!;
        [Column("group_id")]
        [ForeignKey(nameof(Group))]
        public Guid? GroupId { get; set; }

        public CoOwnershipGroup? Group { get; set; }
        public ICollection<EContract> Contracts { get; set; } = new List<EContract>();
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<VehicleRequest> VehicleRequests { get; set; } = new List<VehicleRequest>();
        public ICollection<TripEvent> TripEvents { get; set; } = new List<TripEvent>();

    }
}
