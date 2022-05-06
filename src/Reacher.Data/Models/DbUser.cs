namespace Reacher.Data.Models;
[Table("User")]
public partial class DbUser : BaseModel
{
    [Key]
    public Guid Id { get; set; }
    [MaxLength(Constants.StandardTextLength)]
    public string Name { get; set; }
    [MaxLength(Constants.StandardTextLength)]
    public string EmailAddress { get; set; }
    [MaxLength(100)]
    public string StrikeUsername { get; set; }

    public virtual List<DbWhitelist> Whitelist { get; set; }
}
