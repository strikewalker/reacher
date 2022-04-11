using Reacher.Data.Models;
using StrongGrid;
using StrongGrid.Models.Webhooks;
using SendGrid.Helpers.Mail;

namespace Reacher.Common.Utilities;

public class SendGridParser : ISendGridParser
{
    public async Task<InboundEmail> ParseSendGridInboundEmail(Stream stream)
    {
        var parser = new WebhookParser();
        var inboundEmail = await parser.ParseInboundEmailWebhookAsync(stream);
        inboundEmail.Html = inboundEmail.Html ?? $"<html><pre>{inboundEmail.Text}</pre></html>";
        return inboundEmail;
    }
    public DbEmail MapInboundEmailToDbEmail(InboundEmail inboundEmail)
    {
        var result = new DbEmail();

        var toEmail = inboundEmail.To?.FirstOrDefault();
        result.ToEmailAddress = toEmail?.Email;
        result.ToEmailName = toEmail?.Name;

        var fromEmail = inboundEmail.From;
        result.FromEmailAddress = fromEmail?.Email;
        result.FromEmailName = fromEmail?.Name;

        result.Subject = inboundEmail.Subject;
        return result;
    }
}
public static class AttachmentHelper
{
    public static IEnumerable<Attachment>? GetAttachments(this InboundEmail parsedEmail)
    {
        if (parsedEmail.Attachments?.Any() != true)
            return Array.Empty<Attachment>();
        return parsedEmail.Attachments.Select(a => new Attachment
        {
            Content = a.Data.StreamToBase64(),
            Filename = a.FileName,
            ContentId = a.ContentId,
            Type = a.ContentType,
        });
    }
    public static string StreamToBase64(this Stream input)
    {
        using MemoryStream ms = new();
        input.CopyTo(ms);
        return Convert.ToBase64String(ms.ToArray());
    }
}

public interface ISendGridParser
{
    DbEmail MapInboundEmailToDbEmail(InboundEmail inboundEmail);
    Task<InboundEmail> ParseSendGridInboundEmail(Stream stream);
}