using Azure;
using Azure.Communication.Email;
using hagglehaul.Server.Models;
using Microsoft.Extensions.Options;
using Razor.Templating.Core;

namespace hagglehaul.Server.Services;

public enum EmailNotificationType
{
    NewBid, // Notify rider when a new bid is placed
    AcceptedBid, // Notify driver when their bid is accepted
    Confirmation, // Notify rider with next steps after bid is accepted
}

public interface IEmailNotificationService
{
    Task SendEmailNotification(EmailNotificationType type, string recipient, dynamic data);
}

public class EmailNotificationService : IEmailNotificationService
{
    private readonly EmailSettings _emailSettings;
    private EmailClient _emailClient;
    public EmailNotificationService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
        _emailClient = new EmailClient(_emailSettings.ConnectionString);
    }
    
    protected Dictionary<EmailNotificationType, string> _emailTemplates = new()
    {
        { EmailNotificationType.NewBid, "EmailViews/NewBidEmail.cshtml" },
        { EmailNotificationType.AcceptedBid, "EmailViews/AcceptedBidEmail.cshtml" },
        { EmailNotificationType.Confirmation, "EmailViews/ConfirmationEmail.cshtml" },
    };
    
    protected Dictionary<EmailNotificationType, string> _subjectLines = new()
    {
        { EmailNotificationType.NewBid, "Hagglehaul Trip: You have a new bid" },
        { EmailNotificationType.AcceptedBid, "Hagglehaul Driver Management: Your bid has been accepted" },
        { EmailNotificationType.Confirmation, "Hagglehaul Trip: Ride Confirmation" },
    };

    public async Task SendEmailNotification(EmailNotificationType type, string recipient, dynamic data)
    {
        var htmlContent = await RazorTemplateEngine.RenderAsync(_emailTemplates[type], data);
        EmailSendOperation emailSendOperation = _emailClient.Send(
            WaitUntil.Started,
            senderAddress: _emailSettings.SenderAddress,
            recipientAddress: recipient,
            subject: _subjectLines[type],
            htmlContent: htmlContent
        );
        await emailSendOperation.WaitForCompletionAsync();
    }
}
