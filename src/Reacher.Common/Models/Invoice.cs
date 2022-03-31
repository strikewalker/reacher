namespace Reacher.Common.Models;
public class Invoice
{
    public Guid Id { get; set; }
    public Guid StrikeInvoiceId { get; set; }
    public decimal CostUsd { get; set; }
    public Reachable Reachable { get; set; }
    public bool Paid { get; set; }
}
