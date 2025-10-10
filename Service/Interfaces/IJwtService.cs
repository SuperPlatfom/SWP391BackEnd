using BusinessObject.Models;

namespace Service.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(Account user);
    string GenerateRefreshToken();
}