using Azure;
using Azure.Communication.Email;
using hagglehaul.Server.Models;
using Microsoft.Extensions.Options;

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
        // TODO: Replace with actual paths and use Blazor to render the HTML with data parameter
        { EmailNotificationType.NewBid, "somePath/NewBid.html" },
        { EmailNotificationType.AcceptedBid, "somePath/AcceptedBid.html" },
        { EmailNotificationType.Confirmation, "somePath/Confirmation.html" },
    };

    public async Task SendEmailNotification(EmailNotificationType type, string recipient, dynamic data)
    {
        EmailSendOperation emailSendOperation = _emailClient.Send(
            WaitUntil.Started,
            senderAddress: _emailSettings.SenderAddress,
            recipientAddress: recipient,
            subject: "Test Email",
            htmlContent: "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><title>Ride Booking Confirmation</title></head><body style=\"font-family:Arial,sans-serif;margin:0;padding:0\"><table role=\"presentation\" align=\"center\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"><tr><td style=\"padding:20px 0;text-align:center\"><h1 style=\"margin-bottom:20px\">Ride Booking Confirmation</h1><p>Your ride has been successfully booked!</p><p><strong>Ride Details:</strong></p><ul><li><strong>Date:</strong>February 15, 2024</li><li><strong>Time:</strong>10:00 AM</li><li><strong>Origin:</strong>Your Address</li><li><strong>Destination:</strong>Destination Address</li><li><strong>Driver:</strong>Driver's Name</li><li><strong>Contact:</strong>Driver's Phone Number</li></ul><p>Thank you for choosing Hagglehaul!</p></td></tr></table></body></html>"
        );
        await emailSendOperation.WaitForCompletionAsync();
    }
}
