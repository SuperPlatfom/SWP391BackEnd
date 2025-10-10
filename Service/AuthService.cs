using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Microsoft.Extensions.Configuration;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;

namespace Service;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMailService _mailService;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly string _emailSecureCharacters;
    private readonly IFirebaseStorageService _firebaseStorageService;
    private readonly IIdentityCardRepository _identityCardRepository;

    public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IMailService mailService,
        IJwtService jwtService, IConfiguration configuration, IFirebaseStorageService firebaseStorageService, IIdentityCardRepository identityCardRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mailService = mailService;
        _jwtService = jwtService;
        _configuration = configuration;
        _emailSecureCharacters = _configuration["MailSettings:SecureCharacters"] ?? "";
        _firebaseStorageService = firebaseStorageService;
        _identityCardRepository = identityCardRepository;
    }

    public async Task<string> SeedRolesAsync()
    {
        var existingRoles = await _roleRepository.GetAllAsync();

        if (existingRoles.Any())
        {
            return "Roles are already initialized.";
        }

        var roles = new List<Role>
        {
            new Role { Name = "Admin", Description = "Quản trị hệ thống, quản lý tài khoản, nhóm đồng sở hữu, và các đối tác dịch vụ (garage, đăng kiểm, bảo hiểm)."},
            new Role { Name = "Staff", Description = "Nhân viên vận hành, phụ trách hỗ trợ nhóm đồng sở hữu, xác minh thông tin, và quản lý yêu cầu dịch vụ từ các nhóm." },
            new Role { Name = "CoOwner", Description = "Thành viên đồng sở hữu xe điện, có thể tạo nhóm, xác nhận hợp đồng, đặt lịch sử dụng, tạo yêu cầu dịch vụ và chia sẻ chi phí." }
        };

        foreach (var role in roles)
        {
            await _roleRepository.CreateAsync(role);
        }

        return "Roles initialized successfully.";
    }

    public async Task Register(UserRegistrationRequestModel userDto)
    {
        var duplicateField = await CheckDuplicateFieldsAsync(userDto);
        if (duplicateField != null)
            throw new Exception(duplicateField);

        var customerRole = await _roleRepository.GetByNameAsync("CoOwner");
        if (customerRole == null)
            throw new Exception("CoOwner role does not exist. Please initialize roles first.");

        string verificationCode = GenerateActivationCode();

        string frontImageUrl = userDto.frontImage != null
            ? await _firebaseStorageService.UploadFileAsync(userDto.frontImage, "uploads")
            : string.Empty;

        string backImageUrl = userDto.backImage != null
            ? await _firebaseStorageService.UploadFileAsync(userDto.backImage, "uploads")
            : string.Empty;

        var accountId = Guid.NewGuid();
        var account = new Account
        {
            Id = accountId,
            Email = userDto.email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.password),
            FullName = userDto.fullName,
            DateOfBirth = userDto.dateOfBirth.ToUniversalTime(),
            Phone = userDto.phone,
            Gender = userDto.gender,
            Status = "inactive",
            RoleId = customerRole.Id,
            ImageUrl = "",
            ActivationCode = verificationCode,
            RefreshToken = null,
            RefreshTokenExpiry = null,
            IsLoggedIn = false,
            CreatedAt = DateTime.UtcNow
        };

        var identityCard = new CitizenIdentityCard
        {
            Id = Guid.NewGuid(),
            UserId = accountId,
            IdNumber = userDto.idNumber,
            Address = userDto.address,
            IssueDate = userDto.issueDate.ToUniversalTime(),
            ExpiryDate = userDto.expiryDate.ToUniversalTime(),
            PlaceOfIssue = userDto.placeOfIssue,
            PlaceOfBirth = userDto.placeOfBirth,
            FrontImageUrl = frontImageUrl,
            BackImageUrl = backImageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(account);
        await _identityCardRepository.CreateAsync(identityCard);

        string subject = "Your Verification Code";
        string message = $"<h2>Welcome to Our Service</h2><p>Your verification code is: <b>{verificationCode}</b></p>";
        await _mailService.SendEmailVerificationCode(userDto.email, subject, message);
    }

    private async Task<string?> CheckDuplicateFieldsAsync(UserRegistrationRequestModel userDto)
    {
        if (await _userRepository.GetByEmailAsync(userDto.email) != null)
            return "Email already exists.";

        if (await _userRepository.GetByPhoneAsync(userDto.phone) != null)
            return "Phone number already exists.";

        if (await _userRepository.GetByIdNumberAsync(userDto.idNumber) != null)
            return "Cccd number already exists.";

        return null;
    }

    private string GenerateActivationCode()
    {
        StringBuilder codeBuilder = new StringBuilder();
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            byte[] randomBytes = new byte[1];

            for (int i = 0; i < 6; i++)
            {
                rng.GetBytes(randomBytes);
                int randomIndex = randomBytes[0] % _emailSecureCharacters.Length;
                codeBuilder.Append(_emailSecureCharacters[randomIndex]);
            }
        }

        return codeBuilder.ToString();
    }

    public async Task VerifyOtp(string email, string otp)
    {
        if (string.IsNullOrWhiteSpace(otp))
        {
            throw new Exception("OTP cannot be empty.");
        }
        else if (string.IsNullOrWhiteSpace(email))
        {
            throw new Exception("Email cannot be empty.");
        }

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            throw new KeyNotFoundException("Account not found.");
        }

        if (user.ActivationCode != otp)
        {
            throw new UnauthorizedAccessException("Incorrect OTP. Please try again.");
        }

        user.Status = "active";
        user.ActivationCode = null;
        await _userRepository.UpdateAsync(user);
    }

    public async Task<UserAuthenticationResponse> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new Exception("Email and password cannot be empty.");

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Incorrect email/password please try again.");
        }
        
        if (user.Status.ToLower() == "inactive" && !string.IsNullOrEmpty(user.ActivationCode))
        {
            throw new UnauthorizedAccessException("Your account is not verified.");
        }

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(1);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = refreshTokenExpiry;
        user.IsLoggedIn = true;
        await _userRepository.UpdateAsync(user);

        return new UserAuthenticationResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<UserAuthenticationResponse> BiometricLoginAsync(string email, string password, string deviceId)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new Exception("Email and password cannot be empty.");

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Incorrect email/password please try again.");
        }

        var hashedDeviceId = HashHelper.ComputeSha256Hash(deviceId);


        if (user.Status.ToLower() == "inactive" && !string.IsNullOrEmpty(user.ActivationCode))
        {
            throw new UnauthorizedAccessException("Your account is not verified.");
        }

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(1);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = refreshTokenExpiry;
        user.IsLoggedIn = true;
        await _userRepository.UpdateAsync(user);

        return new UserAuthenticationResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<UserAuthenticationResponse> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UnauthorizedAccessException("Refresh token is required.");

        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        if (user.RefreshTokenExpiry == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token expired, please log in again.");

        var newAccessToken = _jwtService.GenerateAccessToken(user);

        return new UserAuthenticationResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = user.RefreshToken
        };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UnauthorizedAccessException("Refresh token is required.");

        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        if (user.RefreshTokenExpiry == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token expired, please log in again.");

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        user.IsLoggedIn = false;

        await _userRepository.UpdateAsync(user);
    }

    public async Task RequestPasswordResetAsync(ForgotPasswordRequestModel model)
    {
        var account = await _userRepository.GetByEmailAsync(model.Email);
        if (account == null)
            throw new Exception("No account associated with this email.");

        string code = GenerateActivationCode();
        account.ActivationCode = code;
        account.CodeExpiry = DateTime.UtcNow.AddMinutes(10);

        await _userRepository.UpdateAsync(account);

        string subject = "Reset Your Password";
        string message = $"<p>Your password reset code is: <b>{code}</b></p>";
        await _mailService.SendEmailVerificationCode(model.Email, subject, message);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequestModel model)
    {
        var account = await _userRepository.GetByEmailAsync(model.Email);
        if (account == null)
            throw new Exception("Invalid request.");

        if (account.ActivationCode != model.ActivationCode ||
            (account.CodeExpiry != null && account.CodeExpiry < DateTime.UtcNow))
            throw new Exception("Invalid or expired verification code.");

        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        account.ActivationCode = null;
        account.CodeExpiry = null;

        await _userRepository.UpdateAsync(account);
    }

    public async Task SendProfileUpdateOtpAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            throw new Exception("User not found");

        var code = GenerateActivationCode();
        user.ActivationCode = code;
        user.CodeExpiry = DateTime.UtcNow.AddMinutes(10);
        await _userRepository.UpdateAsync(user);

        string subject = "Update Profile Verification";
        string message = $"<p>Your update verification code is: <b>{code}</b></p>";
        await _mailService.SendEmailVerificationCode(email, subject, message);
    }
}