using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BusinessObject.DTOs.RequestModels;

public class UserLoginRequestModel
{
    [JsonPropertyName("email")]
    [Required(ErrorMessage = "Email cannot be blank")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    [Required(ErrorMessage = "Password cannot be blank")]
    public string Password { get; set; } = string.Empty;
}

public class UserBiometricLoginRequestModel
{
    [JsonPropertyName("deviceId")]
    [Required(ErrorMessage = "Device id cannot be blank")]
    public string DeviceId { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    [Required(ErrorMessage = "Email cannot be blank")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    [Required(ErrorMessage = "Password cannot be blank")]
    public string Password { get; set; } = string.Empty;
}