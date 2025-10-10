using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("vehicle")]
    public class Vehicle
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("plate_number")]
        public string? PlateNumber { get; set; }  // Biển số (nếu có)

        [Column("make")]
        public string Make { get; set; }          // Hãng xe

        [Column("model")]
        public string Model { get; set; }         // Dòng xe

        [Column("model_year")]
        public int ModelYear { get; set; }        // Năm sản xuất

        [Column("color")]
        public string Color { get; set; }         // Màu sắc

        [Column("battery_capacity_kwh")]
        public decimal BatteryCapacityKwh { get; set; } // Dung lượng pin danh định

        [Column("range_km")]
        public int RangeKm { get; set; }          // Tầm hoạt động ước tính

        [Column("telematics_device_id")]
        public string? TelematicsDeviceId { get; set; } // Thiết bị/IMEI (nếu có)

        [Column("status")]
        public string Status { get; set; }        // ACTIVE / INACTIVE / MAINTENANCE

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
