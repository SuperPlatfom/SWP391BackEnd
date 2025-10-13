using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BusinessObject.DTOs.RequestModels;

public class UpdateUserRequestUsingBiometricOptionModel
{
    [JsonPropertyName("deviceId")]
    [Required(ErrorMessage = "Device id cannot be blank")]
    public string deviceId { get; set; } = string.Empty;

    [JsonPropertyName("isBiometricEnabled")]
    public bool isBiometricEnabled { get; set; }

    [JsonPropertyName("email")]
    [Required(ErrorMessage = "Email cannot be blank")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string email { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"(84|0[3|5|7|8|9])+([0-9]{8})\b",
        ErrorMessage = "Please enter a valid (+84) phone number")]
    public string phone { get; set; } = string.Empty;

    [JsonPropertyName("frontImage")]
    public IFormFile? frontImage { get; set; }

    [JsonPropertyName("backImage")]
    public IFormFile? backImage { get; set; }
}
public class UpdateUserRequestUsingOtpModel
{
    [JsonPropertyName("email")]
    [Required(ErrorMessage = "Email cannot be blank")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string email { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"(84|0[3|5|7|8|9])+([0-9]{8})\b",
        ErrorMessage = "Please enter a valid (+84) phone number")]
    public string phone { get; set; } = string.Empty;

    [JsonPropertyName("frontImage")]
    public IFormFile? frontImage { get; set; }

    [JsonPropertyName("backImage")]
    public IFormFile? backImage { get; set; }
}
public class UserBasicInfo
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
}
