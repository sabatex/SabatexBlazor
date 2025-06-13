using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

using System.Net;
using System.Security.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;


namespace Sabatex.RadzenBlazor.Server;

// Remove the "else if (EmailSender is IdentityNoOpEmailSender)" block from RegisterConfirmation.razor after updating with a real implementation.
public sealed class IdentityEmailSender : Sabatex.Core.IEmailSender<ApplicationUser>
{
    private readonly IConfiguration Configuration;
    private readonly ILogger<IdentityEmailSender> _logger;

    public IdentityEmailSender(IConfiguration configuration,ILogger<IdentityEmailSender> logger)
    {
        Configuration = configuration;
        _logger = logger;
    }
    async Task SendEmailAsync(string email, string subject, string message)
    {
        var MailServer = Configuration.GetSection("MailServer");
        if (!MailServer.Exists())
        {
            _logger.LogError("MailServer not configured");
            return;
        }


        var pass = MailServer.GetValue<string>("Pass");
        var login = MailServer.GetValue<string>("Login");
        var port = MailServer.GetValue<int>("Port");
        var host = MailServer.GetValue<string>("SMTPHost");


        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress("Identity site", login));
        mailMessage.To.Add(new MailboxAddress("",email));
        mailMessage.Subject = subject;
        mailMessage.Body = new TextPart("plain") { Text = message };



        using (var smtpClient = new SmtpClient())
        {
            await smtpClient.ConnectAsync(host, port,true);
            _logger.LogTrace($"Connect for send email  to {host}:{port}");
            await smtpClient.AuthenticateAsync(login, pass);
            await smtpClient.SendAsync(mailMessage);
            await smtpClient.DisconnectAsync(true);
        };

        //using (var mail = new MailMessage(login, email, subject, message))
        //{
        //    mail.IsBodyHtml = true;
        //    await smtpClient.SendMailAsync(mail);
        //}



    }
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
        SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
}
