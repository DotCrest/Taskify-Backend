using Domain.Contracts;
using Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;
namespace Infrastructure;

public class EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger) : IEmailService
{
    private readonly EmailOptions _emailOptions = options.Value;
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("taskify-team", _emailOptions.Email));
        message.To.Add(new MailboxAddress(new MailAddress(to).User, to));

        message.Subject = subject;

        var bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = $@"
        <div style='max-width: 600px; margin: auto; font-family: Arial, sans-serif; border: 1px solid #eee;'>
            <div style='padding: 20px; color: #333; line-height: 1.6;'>
                {body}
            </div>
            <div style='padding: 10px; text-align: center; font-size: 12px; color: #999; background: #f9f9f9;'>
                © 2025 RecoMind - All Rights Reserved
            </div>
        </div>";
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new MailKit.Net.Smtp.SmtpClient();
        try
        {
            await client.ConnectAsync(_emailOptions.Host, _emailOptions.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailOptions.Email, _emailOptions.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (MailKit.Security.AuthenticationException ex)
        {
            logger.LogError(ex, "Authentication failed: Invalid credentials for {Email}. Message: {Message}", _emailOptions.Email, ex.Message);
            throw;
        }
        catch (MailKit.Net.Smtp.SmtpCommandException ex)
        {
            logger.LogError(ex, "SMTP Command error: Server returned an error code {StatusCode} while sending to {Receiver}. Message: {Message}",
                ex.ErrorCode, message.To, ex.Message);
            throw;
        }
        catch (System.Net.Sockets.SocketException ex)
        {
            logger.LogError(ex, "Network error: Could not connect to SMTP host {Host}:{Port}. Message: {Message}",
                _emailOptions.Host, _emailOptions.Port, ex.Message);
            throw;
        }
        catch (MailKit.Net.Smtp.SmtpProtocolException ex)
        {
            logger.LogError(ex, "Protocol error: An unexpected error occurred during SMTP communication. Message: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error: MailKit SMTP client failed to send email to {Receiver}. Message: {Message}",
                message.To, ex.Message);
            throw;
        }
    }
}
