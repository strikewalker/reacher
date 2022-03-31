using SendGrid;
using SendGrid.Helpers.Mail;

namespace Reacher.Data.External;
public class SendGridFacade : ISendGridFacade
{
    private readonly ISendGridClient _sendGridClient;

    public SendGridFacade(ISendGridClient sendGridClient)
    {
        _sendGridClient = sendGridClient;
    }
    private SendGridMessage getDefaultEmail()
    {
        var mail = new SendGridMessage();
        mail.TrackingSettings = new TrackingSettings
        {
            ClickTracking = new ClickTracking { Enable = true },
            OpenTracking = new OpenTracking() { Enable = true }
        };

        return mail;
    }

    public async Task SendEmail(string toEmail, string? toEmailName, string fromEmail, string? fromName, string subject, string bodyHtml, string? replyToEmail = null)
    {
        var message = getDefaultEmail();

        if (fromName == string.Empty)
            fromName = null;
        message.From = new EmailAddress(fromEmail, fromName);
        if (replyToEmail != null)
        {
            message.ReplyTo = new EmailAddress(replyToEmail, fromName);
        }

        message.AddTo(new EmailAddress(toEmail, toEmailName));
        message.Subject = subject;
        message.AddContent("text/html", bodyHtml);

        await _sendGridClient.SendEmailAsync(message);
    }
}

public interface ISendGridFacade
{
    Task SendEmail(string toEmail, string? toEmailName, string fromEmail, string? fromName, string subject, string bodyHtml, string? replyToEmail = null);
}