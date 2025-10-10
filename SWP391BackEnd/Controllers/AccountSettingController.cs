using System.Net;
using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using SWP391BackEnd.Helpers;
using Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Repository.HandleException;

namespace SWP391BackEnd.Controllers;

[ApiController]
[Route("api/settings")]
[ApiExplorerSettings(GroupName = "Account Settings")]
public class AccountSettingController : Controller
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public AccountSettingController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetUserById()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("User ID claim not found.");

        var userId = Guid.Parse(userIdClaim.Value);
        try
        {
            var user = await _userService.GetUserByIdAsync(userId);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Accepted,
                "Successfully Retrieve Account Information",
                user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("profile/otp")]
    public async Task<IActionResult> UpdateUserUsingOtp([FromForm] UpdateUserRequestUsingOtpModel model, string otp)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

        var userId = Guid.Parse(userIdClaim.Value);

        try
        {
            bool updated = await _userService.UpdateUserUsingOtpAsync(userId, model, otp);
            if (!updated) return NotFound("User not found.");

            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                "Successfully updated account information.", null);
        }
        catch (CustomValidationError ex)
        {
            return CustomErrorHandler.ValidationError(ex.Errors);
        }
        catch (KeyNotFoundException ex)
        {
            return CustomErrorHandler.SimpleError(ex.Message, 404);
        }
        catch (Exception ex)
        {
            return CustomErrorHandler.SimpleError(ex.Message, 500);
        }
    }

    [Authorize]
    [HttpPut("profile/biometric")]
    public async Task<IActionResult> UpdateUserUsingBiometricOption([FromForm] UpdateUserRequestUsingBiometricOptionModel model)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

        var userId = Guid.Parse(userIdClaim.Value);

        try
        {
            bool updated = await _userService.UpdateUserUsingBiometricOptionAsync(userId, model);
            if (!updated) return NotFound("User not found.");

            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                "Successfully updated account information.", null);
        }
        catch (CustomValidationError ex)
        {
            return CustomErrorHandler.ValidationError(ex.Errors);
        }
        catch (KeyNotFoundException ex)
        {
            return CustomErrorHandler.SimpleError(ex.Message, 404);
        }
        catch (Exception ex)
        {
            return CustomErrorHandler.SimpleError(ex.Message, 500);
        }
    }

    [Authorize]
    [HttpPost("request-profile-update-otp")]
    public async Task<IActionResult> RequestProfileUpdateOtp(string email)
    {
        try
        {
            await _authService.SendProfileUpdateOtpAsync(email);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Verification code sent to your email.", null);
        }
        catch (Exception ex)
        {
            return CustomErrorHandler.SimpleError(ex.Message, 400);
        }
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestModel model)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("User ID claim not found.");

        var userId = Guid.Parse(userIdClaim.Value);

        try
        {
            await _userService.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Password changed successfully.", null);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return CustomErrorHandler.SimpleError(ex.Message, 400);
        }
    }


    [Authorize]
    [HttpPost("profile/avatar")]
    public async Task<IActionResult> UploadUserImage(IFormFile file)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("User ID claim not found.");

        var userId = Guid.Parse(userIdClaim.Value);

        try
        {
            var imageUrl = await _userService.UploadAvatarAsync(file);
            bool updated = await _userService.UpdateUserImageAsync(userId, imageUrl);

            if (!updated) return BadRequest("Failed to update.");

            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                "Successfully uploaded and updated profile image", null);
        }
        catch (Exception ex)
        {
            return StatusCode(400, new { message = "An error occurred", error = ex.Message });
        }
    }

    
}