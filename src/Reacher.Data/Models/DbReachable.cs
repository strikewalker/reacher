namespace Reacher.Data.Models;
[Index(nameof(ReacherEmailAddress), IsUnique = true)]
[Table("Reachable")]
public partial class DbReachable : BaseModel
{
    [Key]
    public Guid Id { get; set; }
    [MaxLength(Constants.StandardTextLength)]
    public string Name { get; set; }
    [MaxLength(Constants.StandardTextLength)]
    public string ReacherEmailAddress { get; set; }
    [MaxLength(Constants.StandardTextLength)]
    public string ToEmailAddress { get; set; }
    [MaxLength(100)]
    public string StrikeUsername { get; set; }
    [Precision(18, 2)]
    public decimal CostUsdToReach { get; set; }
    [MaxLength(10)]
    public string? Currency { get; set; }
    public bool Disabled { get; set; }

    [ForeignKey(nameof(User))]
    public Guid? UserId { get; set; }

    public virtual DbUser? User { get; set; }
    public virtual List<DbEmail> Emails { get; set; }
}
