using hagglehaul.Server.EmailViews;
using hagglehaul.Server.Models;
using hagglehaul.Server.Services;
using Microsoft.Extensions.Options;

namespace hagglehaul.Tests.ServiceTests;

public class EmailNotificationServiceTests : ServiceTestsBase
{
    protected IOptions<EmailSettings> _emailSettings;
    /*
     * We do not write any tests that check actual sending of emails due to non-technical reasons.
     * Each outbound email would be charged to the account associated with the connection string.
     */
    protected const string CONNECTION_STRING = "endpoint=https://127.0.0.1/;accesskey=AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA/AAAAAAAAA/AAAAAAAA==";
    protected const string SENDER_ADDRESS = "hagglehaul@example.com";

    [SetUp]
    public void Setup()
    {
        _emailSettings = Options.Create(new EmailSettings {ConnectionString = CONNECTION_STRING, SenderAddress = SENDER_ADDRESS});
    }

    [Test]
    public async Task EmailNotificationFailsWithInvalidConnection()
    {
        var emailNotificationService = new EmailNotificationService(_emailSettings);
        Assert.ThrowsAsync<AggregateException>(async () =>
        {
            await emailNotificationService.SendEmailNotification(EmailNotificationType.Confirmation,
                "test@example.com",
                new ConfirmationEmail());
        });
        Assert.ThrowsAsync<AggregateException>(async () =>
        {
            await emailNotificationService.SendEmailNotification(EmailNotificationType.AcceptedBid,
                "test@example.com",
                new AcceptedBidEmail());
        });
        Assert.ThrowsAsync<AggregateException>(async () =>
        {
            await emailNotificationService.SendEmailNotification(EmailNotificationType.NewBid,
                "test@example.com",
                new NewBidEmail());
        });
    }
}
