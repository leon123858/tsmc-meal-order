using System.Net;
using System.Net.Mail;
using mail.Model;
using Microsoft.Extensions.Options;

namespace mail.Service;

public class MailService
{
    private readonly string _host;
    private readonly ILogger<MailService> _logger;
    private readonly string _senderEmail;
    private readonly string _senderPassword;

    public MailService(IOptions<SMTPSetting> config, ILogger<MailService> logger)
    {
        _logger = logger;
        _senderEmail = config.Value.Sender;
        _host = config.Value.Host;
        _senderPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? config.Value.Password;
    }

    public void SendEmail(MailEvent mailEvent)
    {
        // Create a MailMessage object
        var mailMessage = new MailMessage(_senderEmail, mailEvent.To);
        mailMessage.Subject = mailEvent.Subject;
        mailMessage.Body = mailEvent.Content;

        // Create a SmtpClient object
        var smtpClient = new SmtpClient(_host);
        smtpClient.Port = 587;
        smtpClient.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
        smtpClient.EnableSsl = true;

        try
        {
            // Send the email
            smtpClient.Send(mailMessage);
            _logger.LogInformation("Mail sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError("Mail send error: {error}", ex.Message);
            throw new Exception(ex.Message);
        }
        finally
        {
            // Dispose of objects to release resources
            mailMessage.Dispose();
            smtpClient.Dispose();
        }
    }
}