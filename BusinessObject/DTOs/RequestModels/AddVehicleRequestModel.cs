using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BusinessObject.DTOs.RequestModels
{
    public class AddVehicleRequestModel
    {
        [JsonPropertyName("plateNumber")]
        [RegularExpression(@"^[0-9A-Z\-]{5,15}$", ErrorMessage = "Invalid plate number format")]
        public string? PlateNumber { get; set; }

        [JsonPropertyName("make")]
        [Required(ErrorMessage = "Vehicle make (brand) is required")]
        [MinLength(2, ErrorMessage = "Make must be at least 2 characters")]
        public string Make { get; set; } = string.Empty;

        [JsonPropertyName("model")]
        [Required(ErrorMessage = "Vehicle model is required")]
        [MinLength(1, ErrorMessage = "Model cannot be empty")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("modelYear")]
        [Required(ErrorMessage = "Model year is required")]
        [Range(2000, 2100, ErrorMessage = "Model year must be between 2000 and 2100")]
        public int ModelYear { get; set; }

        [JsonPropertyName("color")]
        [Required(ErrorMessage = "Color is required")]
        [MinLength(2, ErrorMessage = "Color must be at least 2 characters")]
        public string Color { get; set; } = string.Empty;

        [JsonPropertyName("batteryCapacityKwh")]
        [Required(ErrorMessage = "Battery capacity is required")]
        [Range(10, 200, ErrorMessage = "Battery capacity must be between 10 and 200 kWh")]
        public decimal BatteryCapacityKwh { get; set; }

        [JsonPropertyName("rangeKm")]
        [Required(ErrorMessage = "Range is required")]
        [Range(50, 1000, ErrorMessage = "Range must be between 50 and 1000 km")]
        public int RangeKm { get; set; }

        [JsonPropertyName("telematicsDeviceId")]
        [MinLength(5, ErrorMessage = "Telematics Device ID must be at least 5 characters")]
        public string? TelematicsDeviceId { get; set; }

        [JsonPropertyName("status")]
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression(@"^(ACTIVE|INACTIVE|MAINTENANCE)$", ErrorMessage = "Status must be ACTIVE, INACTIVE, or MAINTENANCE")]
        public string Status { get; set; } = "ACTIVE";
    }
}
