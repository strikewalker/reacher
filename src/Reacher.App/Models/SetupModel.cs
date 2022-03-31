namespace Reacher.App.Models;

public record SetupModel
{
    public LoggedInUser User { get; set; }
    public SetupConfig Config { get; set; }
}
public record SetupConfig { 
    public string Name { get; set; }
    public string StrikeUsername { get; set; }
    public decimal Price { get; set; }
    public string? ReacherEmailPrefix { get; set; }
    public string? DestinationEmail { get; set; }
}

public record LoggedInUser { 
    public string Name { get; set; }
    public string StrikeUsername { get; set; }
}
