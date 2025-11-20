using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BusinessObject.RequestModels;

public class VehicleRequestModel
{
    [JsonPropertyName("make")]
    [Required]
    public string make { get; set; } = string.Empty;

    [JsonPropertyName("model")]
    [Required]
    public string model { get; set; } = string.Empty;

    [JsonPropertyName("modelYear")]
    [Required, Range(1900, 2100, ErrorMessage = "Invalid model year")]
    public int modelYear { get; set; }

    [JsonPropertyName("color")]
 
    public string? color { get; set; } = string.Empty;

    [JsonPropertyName("batteryCapacityKwh")]
    [ Range(0, 500, ErrorMessage = "Battery capacity must be positive")]
    public decimal? batteryCapacityKwh { get; set; }

    [JsonPropertyName("rangeKm")]
    [ Range(0, int.MaxValue, ErrorMessage = "Range must be positive")]
    public int? rangeKm { get; set; }

    [JsonPropertyName("plateNumber")]
    [Required]
    public string plateNumber { get; set; }

    [JsonPropertyName("vehicleImage")]
    [Required]
    public List<IFormFile>  vehicleImage { get; set; }

    [JsonPropertyName("registrationPaperUrl")]
    [Required]
    public IFormFile registrationPaperUrl { get; set; }
}


 






