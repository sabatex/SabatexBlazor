using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

using System.Net;
using System.Security.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;


namespace Sabatex.RadzenBlazor.Server;

/// <summary>
/// Provides an implementation of the email sender for identity operations, enabling the sending of confirmation and
/// password reset emails to users.
/// </summary>
/// <remarks>This class is typically used by identity workflows to deliver account-related emails, such as
/// confirmation links and password reset instructions. It relies on mail server configuration settings provided via the
/// application's configuration system. Ensure that the required mail server settings are present and valid before using
/// this sender. This class is sealed and cannot be inherited.</remarks>
public sealed class IdentityEmailSender : Sabatex.Core.Identity.IEmailSender<ApplicationUser>
{
    private readonly IConfiguration Configuration;
    private readonly ILogger<IdentityEmailSender> _logger;
    /// <summary>
    /// Initializes a new instance of the IdentityEmailSender class using the specified configuration and logger.
    /// </summary>
    /// <param name="configuration">The configuration settings used to retrieve email-related options and credentials.</param>
    /// <param name="logger">The logger instance used to record email sending operations and errors.</param>
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
    /// <summary>
    /// Sends an email containing a confirmation link to the specified user.
    /// </summary>
    /// <remarks>The confirmation email includes a clickable link that directs the user to confirm their
    /// account. This method does not validate the email address or confirmation link; callers are responsible for
    /// ensuring valid input.</remarks>
    /// <param name="user">The user to whom the confirmation email will be sent. Must not be null.</param>
    /// <param name="email">The email address of the recipient. Must be a valid, non-empty email address.</param>
    /// <param name="confirmationLink">The URL that the user should visit to confirm their account. Must be a valid, non-empty link.</param>
    /// <returns>A task that represents the asynchronous operation of sending the confirmation email.</returns>
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
        SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
    /// <summary>
    /// Sends a password reset email to the specified user with a link to reset their password.
    /// </summary>
    /// <param name="user">The user for whom the password reset email is being sent. Cannot be null.</param>
    /// <param name="email">The email address to which the password reset link will be sent. Must be a valid, non-empty email address.</param>
    /// <param name="resetLink">The URL that the user can use to reset their password. Must be a valid, non-empty link.</param>
    /// <returns>A task that represents the asynchronous operation of sending the password reset email.</returns>
    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");
    /// <summary>
    /// Sends a password reset code to the specified email address for the given user asynchronously.
    /// </summary>
    /// <param name="user">The user for whom the password reset code is being sent. Cannot be null.</param>
    /// <param name="email">The email address to which the password reset code will be sent. Must be a valid, non-empty email address.</param>
    /// <param name="resetCode">The password reset code to include in the email. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation of sending the password reset email.</returns>
    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
}
