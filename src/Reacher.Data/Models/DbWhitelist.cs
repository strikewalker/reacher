namespace Reacher.Data.Models;
[Index(nameof(EmailAddress), nameof(UserId), IsUnique = true)]
[Table("Whitelist")]
public partial class DbWhitelist: BaseModel
{
    [Key]
    public Guid Id { get; set; }

    public string EmailAddress { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public virtual DbUser User { get; set; }
}
