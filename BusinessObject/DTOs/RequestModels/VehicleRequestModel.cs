using System.ComponentModel.DataAnnotations;

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
        public string? TelematicsDeviceId { get; set; }

    }
    
}
