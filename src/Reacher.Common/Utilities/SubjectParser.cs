using System.Text.RegularExpressions;

namespace Reacher.Common.Utilities;
public static class SubjectParser
{
    private static readonly Regex _subjectFixer = new Regex("([\\[\\(] *)?(RE|FWD?) *([-:;)\\]][ :;\\])-]*|$)|\\]+ *$", RegexOptions.IgnoreCase);
    public static ParsedSubject GetParsedSubject(string originalSubject)
    {
        var cleaned = _subjectFixer.Replace(originalSubject ?? "", "");
        var split = cleaned.Split('|').Select(s => s.Trim());
        var emailId = split.Count() > 1 ? split.Last() : null;
        Guid? emailGuid = null;
        if (!string.IsNullOrWhiteSpace(emailId) && Guid.TryParse(emailId, out var parsedEmailId))
        {
            emailGuid = parsedEmailId;

        }
        return new() { Subject = cleaned, OriginalEmailId = emailGuid };
    }
}
public class ParsedSubject
{
    public string Subject { get; set; }
    public Guid? OriginalEmailId { get; set; }
}
