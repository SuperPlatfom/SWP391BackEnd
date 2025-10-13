using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BusinessObject.DTOs.RequestModels
{
    public class UpdateVehicleRequestModel
    {
        [JsonPropertyName("plateNumber")]
        [RegularExpression(@"^[0-9A-Z\-]{5,15}$", ErrorMessage = "Invalid plate number format")]
        public string? PlateNumber { get; set; }

        [JsonPropertyName("make")]
        [Required(ErrorMessage = "Vehicle make is required")]
        public string Make { get; set; } = string.Empty;

        [JsonPropertyName("model")]
        [Required(ErrorMessage = "Vehicle model is required")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("modelYear")]
        [Range(2000, 2100, ErrorMessage = "Model year must be between 2000 and 2100")]
        public int ModelYear { get; set; }

        [JsonPropertyName("color")]
        [Required(ErrorMessage = "Color is required")]
        public string Color { get; set; } = string.Empty;

        [JsonPropertyName("batteryCapacityKwh")]
        [Range(10, 200, ErrorMessage = "Battery capacity must be between 10 and 200 kWh")]
        public decimal BatteryCapacityKwh { get; set; }

        [JsonPropertyName("rangeKm")]
        [Range(50, 1000, ErrorMessage = "Range must be between 50 and 1000 km")]
        public int RangeKm { get; set; }

        [JsonPropertyName("telematicsDeviceId")]
        public string? TelematicsDeviceId { get; set; }

        [JsonPropertyName("status")]
        [RegularExpression(@"^(ACTIVE|INACTIVE|MAINTENANCE)$", ErrorMessage = "Status must be ACTIVE, INACTIVE, or MAINTENANCE")]
        public string Status { get; set; } = "ACTIVE";
    }
}
