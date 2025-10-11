using System.ComponentModel.DataAnnotations;

namespace BusinessObject.DTOs.RequestModels;

public class VerifyOtpRequestModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "OTP cannot be empty.")]
    public string Otp { get; set; } = string.Empty;
}
public class ResendOtpRequestModel
{
    public string Email { get; set; } = string.Empty;
}
