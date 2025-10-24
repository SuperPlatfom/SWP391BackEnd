using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BusinessObject.RequestModels
{
    public class VehicleRequestModel
    {

        [Required(ErrorMessage = "Make is required")]
        public string Make { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model is required")]
        public string Model { get; set; } = string.Empty;

        [Range(1900, 2100, ErrorMessage = "Invalid model year")]
        public int ModelYear { get; set; }

        [Required(ErrorMessage = "Color is required")]
        public string Color { get; set; } = string.Empty;

        [Range(0.1, double.MaxValue, ErrorMessage = "Battery capacity must be positive")]
        public decimal BatteryCapacityKwh { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Range must be positive")]
        public int RangeKm { get; set; }

        public string? PlateNumber { get; set; }


    }
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



    }

    public class AttachVehicleRequest
    {
        public Guid GroupId { get; set; }
        public Guid VehicleId { get; set; }
    }
}
