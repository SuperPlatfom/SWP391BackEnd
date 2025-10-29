using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using Repository.HandleException;
using Repository.Repositories;
using Repository;

namespace Service
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IIdentityCardRepository _identityCardRepository;
        private readonly IMailService _mailService;
        private readonly IRoleRepository _roleRepository;
        private readonly IFirebaseStorageService _firebaseStorageService;
        public AccountService(IAccountRepository accountRepository, IUserRepository userRepository, 
            IMailService mailService, IRoleRepository roleRepository,IIdentityCardRepository identityCardRepository, IFirebaseStorageService firebaseStorageService)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _mailService = mailService;
            _identityCardRepository = identityCardRepository;
            _roleRepository = roleRepository;
            _firebaseStorageService = firebaseStorageService;
        }
        public async Task AddAsync(AddAccountRequestModel userDto, string roleName)
        {
            var errors = new Dictionary<string, string>();

            var duplicateField = await CheckDuplicateFieldsAsync(userDto, userDto.idNumber);
            if (duplicateField != null)
            {
                errors["duplicateField"] = duplicateField;
            }

            if (errors.Any())
                throw new CustomValidationError(errors);

            var targetRole = await _roleRepository.GetByNameAsync(roleName);
            if (targetRole == null)
                throw new Exception($"{roleName} role does not exist. Please initialize roles first.");

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
                Status = "active",
                RoleId = targetRole.Id,
                ImageUrl = "",
                ActivationCode = null,
                RefreshToken = null,
                RefreshTokenExpiry = null,
                IsLoggedIn = false,
                CreatedAt = DateTime.UtcNow
            };

            string frontImageUrl = userDto.frontImage != null
                ? await _firebaseStorageService.UploadFileAsync(userDto.frontImage, "uploads")
                : string.Empty;

            string backImageUrl = userDto.backImage != null
                ? await _firebaseStorageService.UploadFileAsync(userDto.backImage, "uploads")
                : string.Empty;
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

            await _accountRepository.AddAsync(account);
            await _identityCardRepository.CreateAsync(identityCard);

            string subject = $"Thông tin tài khoản {roleName}";
            string message = $@"
        <p>Kính chào anh/chị,</p>
        <p>Hệ thống <strong>EV Sharing</strong> đã khởi tạo tài khoản <strong>{roleName}</strong>.</p>
        <p><strong>Thông tin đăng nhập:</strong></p>
        <ul>
            <li><strong>Email:</strong> {userDto.email}</li>
            <li><strong>Mật khẩu:</strong> {userDto.password}</li>
        </ul>
        <p>Vui lòng đăng nhập và đổi mật khẩu ngay sau lần đăng nhập đầu tiên để đảm bảo bảo mật.</p>
        <p>Trân trọng,<br/>Hệ thống EV Sharing</p>";

            await _mailService.SendEmailVerificationCode(userDto.email, subject, message);
        }


        private async Task<string?> CheckDuplicateFieldsAsync(AddAccountRequestModel userDto, string idNumber)
        {
            if (await _userRepository.GetByEmailAsync(userDto.email) != null)
                return "Email already exists.";

            if (await _userRepository.GetByPhoneAsync(userDto.phone) != null)
                return "Phone number already exists.";

            if (await _userRepository.GetByIdNumberAsync(idNumber) != null)
                return "Cccd number already exists.";

            return null;
        }

        public async Task<IEnumerable<AccountResponseModel>> GetAllAsync(string? role = null)
        {
            var accounts = await _accountRepository.GetByRoleAsync(role);

            return accounts.Select(a => new AccountResponseModel
            {
                Id = a.Id,
                FullName = a.FullName,
                Email = a.Email,
                Phone = a.Phone,
                Gender = a.Gender,
                DateOfBirth = a.DateOfBirth,
                Status = a.Status,
                ImageUrl = a.ImageUrl,
                RoleName = a.Role?.Name ?? "",
                CreatedAt = a.CreatedAt,
                CitizenIdNumber = a.CitizenIdentityCard?.IdNumber,
                Address = a.CitizenIdentityCard?.Address,
                PlaceOfIssue = a.CitizenIdentityCard?.PlaceOfIssue,
                IssueDate = a.CitizenIdentityCard?.IssueDate,
                ExpiryDate = a.CitizenIdentityCard?.ExpiryDate,
                FrontImageUrl = a.CitizenIdentityCard?.FrontImageUrl,
                BackImageUrl = a.CitizenIdentityCard?.BackImageUrl,
            }).ToList();
        }


    }
}
