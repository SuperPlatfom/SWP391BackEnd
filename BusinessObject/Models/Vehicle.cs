
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BusinessObject.Models
{
    [Table("vehicle")]
    public class Vehicle
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("plate_number")]
        public string? PlateNumber { get; set; }  // Biển số 

        [Column("make")]
        public string? Make { get; set; }          // Hãng xe

        [Column("model")]
        public string? Model { get; set; }         // Dòng xe

        [Column("model_year")]
        public int ModelYear { get; set; }        // Năm sản xuất

        [Column("color")]
        public string? Color { get; set; }         // Màu sắc

        [Column("battery_capacity_kwh")]
        public decimal BatteryCapacityKwh { get; set; } // Dung lượng pin danh định

        [Column("range_km")]
        public int RangeKm { get; set; }          // Tầm hoạt động ước tính

        [Column("status")]
        public string Status { get; set; } = "INACTIVE";        // ACTIVE / INACTIVE / MAINTENANCE

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
    }
}
