using Reacher.Data.Models;
using StrongGrid;

namespace Reacher.Common.Utilities;

public class SendGridParser : ISendGridParser
{
    public async Task<DbEmail> ParseSendGridInboundEmail(Stream stream)
    {
        var parser = new WebhookParser();
        var inboundEmail = await parser.ParseInboundEmailWebhookAsync(stream);
        var result = new DbEmail();

        var toEmail = inboundEmail.To?.FirstOrDefault();
        result.ToEmailAddress = toEmail?.Email;
        result.ToEmailName = toEmail?.Name;

        var fromEmail = inboundEmail.From;
        result.FromEmailAddress = fromEmail?.Email;
        result.FromEmailName = fromEmail?.Name;

        result.Body = inboundEmail.Html ?? $"<html><pre>{inboundEmail.Text}</pre></html>";
        result.Subject = inboundEmail.Subject;
        return result;
    }
}

public interface ISendGridParser
{
    Task<DbEmail> ParseSendGridInboundEmail(Stream stream);
}