namespace Reacher.Data.Models;
[Index(nameof(StrikeInvoiceId))]
[Index(nameof(SentDate))]
[Index(nameof(ToEmailAddress))]
[Index(nameof(FromEmailAddress))]
[Table("Email")]
public partial class DbEmail : BaseModel
{
    [Key]
    public Guid Id { get; set; }
    [ForeignKey(nameof(TypeRef)), DefaultValue(EmailType.New)]
    public EmailType Type { get; set; }
    [MaxLength(Constants.MaxSubjectLength)]
    public string Subject { get; set; }
    [MaxLength(Constants.MaxEmailLength)]
    public string FromEmailAddress { get; set; }
    [MaxLength(Constants.StandardTextLength)]
    public string? FromEmailName { get; set; }
    [MaxLength(Constants.MaxEmailLength)]
    public string ToEmailAddress { get; set; }
    [MaxLength(Constants.StandardTextLength)]
    public string? ToEmailName { get; set; }
    public DateTime? SentDate { get; set; }

    [ForeignKey(nameof(OriginalEmail))]
    public Guid? OriginalEmailId { get; set; }

    [ForeignKey(nameof(Reachable))]
    public Guid? ReachableId { get; set; }
    [ForeignKey(nameof(InvoiceStatusRef))]
    public InvoiceStatus? InvoiceStatus { get; set; }
    [Precision(18, 2)]
    public decimal? CostUsd { get; set; }
    public DateTime? PaidDate { get; set; }
    public Guid? StrikeInvoiceId { get; set; }
    public long? ContentLength { get; set; }

    //references
    public virtual EnumTable<EmailType> TypeRef { get; set; }
    public virtual DbReachable? Reachable { get; set; }
    public virtual DbEmail? OriginalEmail { get; set; }
    public virtual EnumTable<InvoiceStatus>? InvoiceStatusRef { get; set; }
}
