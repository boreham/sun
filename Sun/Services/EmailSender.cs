using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Sun.Services;

public class EmailSender : IEmailSender
{
    private readonly string _smtpServer;
    private readonly string _smtpUser;
    private readonly string _smtpPassword;
    private readonly int _smtpPort;

    public EmailSender(IConfiguration configuration)
    {
        _smtpServer = configuration["EmailSettings:SmtpServer"];
        _smtpUser = configuration["EmailSettings:SmtpUser"];
        _smtpPassword = configuration["EmailSettings:SmtpPassword"];
        _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var fromAddress = new MailAddress(_smtpUser, "Your Application Name");
        var toAddress = new MailAddress(toEmail);
        using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
        {
            smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
            smtpClient.EnableSsl = true;

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            await smtpClient.SendMailAsync(message);
        }
    }
}