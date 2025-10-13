namespace Service.Interfaces;

public interface IMailService
{
    Task SendEmailVerificationCode(string email, string subject, string message);
}