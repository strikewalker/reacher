using System.ComponentModel.DataAnnotations;

namespace Reacher.App.Models;

public record UserModel
{
    public LoggedInUser User { get; set; }
    public SetupConfig Config { get; set; }
    public Whitelist Whitelist { get; set; }
    public List<RecentSender> RecentSenders { get; set; }
}

public record RecentSender
{
    public string FromName { get; set; }
    public string EmailAddress { get; set; }
    public int MessageCount { get; set; }
}


public record Whitelist {
    public string EmailAddresses { get; set; }
}

public record SetupConfig
{
    public string? Name { get; set; }
    public string? StrikeUsername { get; set; }
    public decimal Price { get; set; }
    public string? ReacherEmailPrefix { get; set; }
    public string? DestinationEmail { get; set; }
    public string? Currency { get; set; }
    public bool Disabled { get; set; }
}

public record LoggedInUser
{
    public string Name { get; set; }
    public string StrikeUsername { get; set; }
}
