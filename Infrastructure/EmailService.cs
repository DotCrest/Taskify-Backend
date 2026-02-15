using Domain.Contracts;
using Domain.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Polly;
using Polly.Retry;
using System.Net.Sockets;
namespace Infrastructure;

public class EmailService : IEmailService
{
    public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
    {
        _emailOptions = options.Value;
        _logger = logger;
        // Define a retry policy 
        _retryPolicy = Policy
           .Handle<SocketException>()
           .Or<SmtpProtocolException>()
           .WaitAndRetryAsync
           (
               3,
               retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
               (exception, timeSpan, retryCount, context) => _logger.LogWarning("Retry {Count} triggered due to {Ex.Message}", retryCount, exception.Message)
           );
    }
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<EmailService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = new MimeMessage();
        // sender and receiver
        message.From.Add(new MailboxAddress("taskify-team", _emailOptions.Email));
        var receiverName = to.Split("@")[0];
        message.To.Add(new MailboxAddress(receiverName, to));
        // subject
        message.Subject = subject;
        // body
        var bodyBuilder = new BodyBuilder() { HtmlBody = GenerateTemplate(body) };
        message.Body = bodyBuilder.ToMessageBody();

        // execute send with retry policy
        await _retryPolicy.ExecuteAsync(async () =>
        {
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
                _logger.LogError(ex, "Authentication failed: Invalid credentials for {Email}. Message: {Message}", _emailOptions.Email, ex.Message);
                throw;
            }
            catch (SmtpCommandException ex)
            {
                _logger.LogError(ex, "SMTP Command error: Server returned an error code {StatusCode} while sending to {Receiver}. Message: {Message}",
                    ex.ErrorCode, message.To, ex.Message);
                throw;
            }
            catch (SocketException ex)
            {
                _logger.LogError(ex, "Network error: Could not connect to SMTP host {Host}:{Port}. Message: {Message}",
                    _emailOptions.Host, _emailOptions.Port, ex.Message);
                throw;
            }
            catch (SmtpProtocolException ex)
            {
                _logger.LogError(ex, "Protocol error: An unexpected error occurred during SMTP communication. Message: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error: MailKit SMTP client failed to send email to {Receiver}. Message: {Message}",
                    message.To, ex.Message);
                throw;
            }
        });
    }

    private string GenerateTemplate(string body)
    {
        return
            $@"
        <div style='max-width: 600px; margin: auto; font-family: Arial, sans-serif; border: 1px solid #eee;'>
            <div style='padding: 20px; color: #333; line-height: 1.6;'>
                {body}
            </div>
            <div style='padding: 10px; text-align: center; font-size: 12px; color: #999; background: #f9f9f9;'>
                © {DateTime.Now.Year} RecoMind - All Rights Reserved
            </div>
        </div>";
    }
}
