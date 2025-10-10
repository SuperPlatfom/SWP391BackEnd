using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;

namespace Service;

public class MailService : IMailService
{
    private readonly IConfiguration _configuration;

    public MailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailVerificationCode(string email, string subject, string message)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        string smtpHost = smtpSettings["Server"];
        int smtpPort = int.Parse(smtpSettings["Port"]);
        string smtpUser = smtpSettings["Username"];
        string smtpPass = smtpSettings["Password"];
        bool enableSSL = bool.Parse(smtpSettings["EnableSSL"]);

        using (var client = new SmtpClient(smtpHost, smtpPort))
        {
            client.Credentials = new NetworkCredential(smtpUser, smtpPass);
            client.EnableSsl = enableSSL;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
}