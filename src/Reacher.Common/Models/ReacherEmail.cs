using Markdig;
using RazorEngine.Text;

namespace Reacher.Common.Models;
public class ReacherEmail
{
    public string Title { get; set; }
    public string? Subtitle { get; internal set; }
    public string[]? MarkdownContentItems { get; set; }
    public string[]? MarkdownContentItemsAfter { get; set; }
    public List<EmailAction>? Actions { get; set; }
    public string? BottomNote { get; set; }
    public string? BottomHeader { get; set; }
    public EmailContent? OtherEmail { get; set; }
    public string? Summary { get; set; }
    public bool Invite { get; set; }
}

public class EmailAction
{
    public string ActionName { get; set; }
    public string Url { get; set; }
}

public class RazorReacherEmail
{
    public RazorReacherEmail(ReacherEmail from)
    {
        Title = from.Title;
        Subtitle = from.Subtitle;
        Actions = from.Actions;
        ContentItems = from.MarkdownContentItems?.Select(MarkdownHelper.ToHtml).ToArray();
        ContentItemsAfter = from.MarkdownContentItemsAfter?.Select(MarkdownHelper.ToHtml).ToArray();
        BottomNote = from.BottomNote.ToHtml();
        BottomHeader = from.BottomHeader.ToHtml();
        OtherEmail = from.OtherEmail;
        if (OtherEmail != null)
        {
            OtherEmail.Label = OtherEmail.Label.ToHtml();
        }
        Summary = from.Summary.ToHtml();
        Invite = from.Invite;
    }
    public string? Subtitle { get; set; }
    public string Title { get; set; }
    public EmailContent? OtherEmail { get; set; }
    public string[]? ContentItems { get; set; }
    public object[]? ContentItemsAfter { get; set; }
    public List<EmailAction>? Actions { get; set; }
    public string? BottomNote { get; set; }
    public bool Invite { get; set; }
    public string? BottomHeader { get; set; }
    public string? Summary { get; set; }
}

public class EmailContent
{
    public string Subject { get; set; }
    public string Label { get; set; }
    public string Html { get; set; }
    public string? From { get; set; }
}

public static class MarkdownHelper
{
    const string BREAK = "<br/>";
    private static MarkdownPipeline? pipeline = new MarkdownPipelineBuilder().ConfigureNewLine(BREAK).Build();
    public static string? ToHtml(this string? markdownString)
    {
        if (markdownString == null)
            return null;
        var html = Markdown.ToHtml(markdownString, pipeline);
        if (html.EndsWith(BREAK))
            html = html.Remove(html.Length - BREAK.Length);
        return html;
    }
}