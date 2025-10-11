using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using Service;
using BusinessObject.DTOs.ResponseModels;
using SWP391BackEnd.Helpers;

namespace SWP391BackEnd.Controllers;

[ApiController]
[Route("api/auth")]
[ApiExplorerSettings(GroupName = "Authentication")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IAccountService _accountService;

    private readonly IConfiguration _configuration;
    private readonly IScanningCardService _scanningCardService;
    public AuthController(IAuthService service, IConfiguration configuration, IAccountService accountService, IScanningCardService scanningCardService)
    {
        _authService = service;
        _configuration = configuration;
        _accountService = accountService;
        _scanningCardService = scanningCardService;
    }

    [HttpGet("init-roles")]
    public async Task<IActionResult> InitializeRoles()
    {
        var result = await _authService.SeedRolesAsync();
        return Ok(result);
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm]UserRegistrationRequestModel userDto)
    {
        try
        {
            await _authService.Register(userDto);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Accepted, "Successfully Register",
            "Please check your email for account verification.");
        }
        catch (Exception ex)
        {
            return CustomErrorHandler.SimpleError(ex.Message, 500);
        }       
    }

    [HttpPost("identity-card")]
    public async Task<IActionResult> ScanIdCard(IFormFile file)
    {
        var parsedResult = await _scanningCardService.ParseVietnameseIdCardAsync(file);

        if (parsedResult == null)
            return BadRequest("Failed to scan or parse ID card.");

        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Accepted, "Successfully scanned identity card", parsedResult);
    }



    [HttpPost("verify-account")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestModel request)
    {
        try
        {
            await _authService.VerifyOtp(request.Email, request.Otp);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "OTP Verified",
                "Your account is now active.");
        } catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        } catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequestModel request)
    {
        try
        {
            await _authService.ResendOtpAsync(request.Email);

            return CustomSuccessHandler.ResponseBuilder(
                HttpStatusCode.OK,
                "OTP Resent Successfully",
                "A new verification code has been sent to your email."
            );
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


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestModel request)
    {
        try
        {
            var authResponse = await _authService.LoginAsync(request.Email, request.Password);

            var responseObject = new
            {
                accessToken = authResponse.AccessToken,
                refreshToken = authResponse.RefreshToken
            };

            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Login successful", responseObject);
        } catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }



    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestModel request)
    {
        try
        {
            var authResponse = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(new
            {
                accessToken = authResponse.AccessToken
            });
        } catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        } catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while refreshing the token.", error = ex.Message });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestModel request)
    {
        try
        {
            await _authService.LogoutAsync(request.RefreshToken);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Logout successful",
                "You have been logged out.");
        } catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        } catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while logging out.", error = ex.Message });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestModel model)
    {
        try
        {
            await _authService.RequestPasswordResetAsync(model);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Verification code sent to your email.", null);
        }
        catch (Exception ex)
        {
            return CustomErrorHandler.SimpleError(ex.Message, 400);
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel model)
    {
        try
        {
            await _authService.ResetPasswordAsync(model);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Password has been reset successfully.", null);
        }
        catch (Exception ex)
        {
            return CustomErrorHandler.SimpleError(ex.Message, 400);
        }
    }

}