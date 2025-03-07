using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Homespirations.Core.Entities;
using Homespirations.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Homespirations.Infrastructure.Services;

public class SesEmailService(ILogger<SesEmailService> logger, IAmazonSimpleEmailService emailService) : IEmailService
{
    private readonly ILogger<SesEmailService> _logger = logger;
    private readonly IAmazonSimpleEmailService _emailService = emailService;

    public async Task SendEmailAsync(EmailOptions emailOption)
    {
        _logger.LogDebug("Email request to {TO}, from {FROM}", emailOption.To, emailOption.From);

        var mailBody = new Body
        {
            Html = new Content { Charset = "UTF-8", Data = emailOption.HtmlBody },
            Text = new Content { Charset = "UTF-8", Data = emailOption.TextBody }
        };

        var message = new Message(new Content(emailOption.Subject), mailBody);
        var request = new SendEmailRequest(emailOption.From, new Destination([emailOption.To]), message);

        try
        {
            await _emailService.SendEmailAsync(request);
            _logger.LogInformation("Email sent successfully to {To}", emailOption.To);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", emailOption.To);
            throw;
        }
    }
}
