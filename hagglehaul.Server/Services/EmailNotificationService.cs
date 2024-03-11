using Azure;
using Azure.Communication.Email;
using hagglehaul.Server.Models;
using Microsoft.Extensions.Options;
using Razor.Templating.Core;

namespace hagglehaul.Server.Services;

/// <summary>
/// The type of email notification to send from the <see cref="EmailNotificationService"/>.
/// </summary>
public enum EmailNotificationType
{
    /// <summary>
    /// An email that notifies the rider when a new bid is placed.
    /// </summary>
    NewBid,
    /// <summary>
    /// An email that notifies the driver when their bid is accepted.
    /// </summary>
    AcceptedBid,
    /// <summary>
    /// An email that notifies the rider with next steps after bid is accepted.
    /// </summary>
    Confirmation,
}

/// <summary>
/// Service to send email notifications through Azure Communication Service.
/// </summary>
public interface IEmailNotificationService
{
    /// <summary>
    /// Sends an email notification of a particular email type to the recipient.
    /// </summary>
    /// <param name="type">The type of notification to send</param>
    /// <param name="recipient">The valid email address of the recipient</param>
    /// <param name="data">The data contained within the email, to be passed to the
    /// email view. Note that the data must be of one of the following types: <br/>
    /// <list type="bullet">
    /// <item><description><see cref="NewBidEmail"/></description></item>
    /// <item><description><see cref="ConfirmationEmail"/></description></item>
    /// <item><description><see cref="AcceptedBidEmail"/></description></item>
    /// </list>
    /// </param>
    /// <returns></returns>
    Task SendEmailNotification(EmailNotificationType type, string recipient, dynamic data);
}

/// <summary>
/// See <see cref="IEmailNotificationService"/>.
/// </summary>
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
