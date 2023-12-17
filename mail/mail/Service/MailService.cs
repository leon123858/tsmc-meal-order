using System;
using System.Collections.Generic;
using Azure;
using Azure.Communication.Email;
using mail.Model;
using Microsoft.Extensions.Options;

namespace mail.Service;

public class MailService
{
    private readonly string _host;
    private readonly ILogger<MailService> _logger;
    private readonly string _senderEmail;
    private readonly string _senderPassword;
    private readonly string _connectionString;

    public MailService(IOptions<SMTPSetting> config, ILogger<MailService> logger)
    {
        _logger = logger;
        _senderEmail = config.Value.Sender;
        _host = config.Value.Host;
        _senderPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? config.Value.Password;
        _connectionString = "endpoint=" + _host +";accesskey=" + _senderPassword;
    }

    public void SendEmail(MailEvent mailEvent)
    {
        try
        {
            new EmailClient(_connectionString).Send(
                WaitUntil.Completed,
                _senderEmail,
                mailEvent.To,
                mailEvent.Subject,
                htmlContent: null,
                mailEvent.Content);
        }
        catch (Exception ex)
        {
            _logger.LogError("Mail send error: {error}", ex.Message);
            throw new Exception(ex.Message);
        }
    }
}