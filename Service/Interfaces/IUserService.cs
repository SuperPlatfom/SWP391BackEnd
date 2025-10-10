using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using Microsoft.AspNetCore.Http;
using static Google.Apis.Requests.BatchRequest;

namespace Service.Interfaces;

public interface IUserService
{
    Task<UserProfileResponseModel?> GetUserByIdAsync(Guid id);
    Task<bool> UpdateUserUsingOtpAsync(Guid id, UpdateUserRequestUsingOtpModel model, string otp);
    Task<bool> UpdateUserUsingBiometricOptionAsync(Guid id, UpdateUserRequestUsingBiometricOptionModel model);
    Task<bool> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);
    Task<bool> UpdateUserImageAsync(Guid id, string imageUrl);
    Task<string> UploadAvatarAsync(IFormFile avatarFile);
    Task<string> UpdateBiometricSettingAsync(Guid userId, BiometricActivationRequest dto);
}