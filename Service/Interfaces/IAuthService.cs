using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;


namespace Service.Interfaces;

public interface IAuthService
{
    Task<string> SeedRolesAsync();
    Task Register(UserRegistrationRequestModel userDto);
    Task VerifyOtp(string email, string otp);
    Task LogoutAsync(string refreshToken);
    Task<UserAuthenticationResponse> LoginAsync(string email, string password);
    Task<UserAuthenticationResponse> BiometricLoginAsync(string email, string password, string deviceId);
    Task<UserAuthenticationResponse> RefreshTokenAsync(string refreshToken);
    Task RequestPasswordResetAsync(ForgotPasswordRequestModel model);
    Task ResetPasswordAsync(ResetPasswordRequestModel model);
    Task SendProfileUpdateOtpAsync(string email);
}