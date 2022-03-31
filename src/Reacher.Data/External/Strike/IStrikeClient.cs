using Reacher.Data.External.Strike.Models;

namespace Reacher.Data.External.Strike;
public interface IStrikeClient
{
    string BaseUrl { get; set; }

    Task<WebhookEventPageResult> Events2Async(string filter, string orderby, double? skip, double? top);
    Task<WebhookEventPageResult> Events2Async(string filter, string orderby, double? skip, double? top, CancellationToken cancellationToken);
    Task<WebhookEvent> EventsAsync(Guid eventId);
    Task<WebhookEvent> EventsAsync(Guid eventId, CancellationToken cancellationToken);
    Task<Invoice> HandleAsync(string handle, CreateInvoiceReq body);
    Task<Invoice> HandleAsync(string handle, CreateInvoiceReq body, CancellationToken cancellationToken);
    Task<Invoice> InvoicesGET2Async(Guid invoiceId);
    Task<Invoice> InvoicesGET2Async(Guid invoiceId, CancellationToken cancellationToken);
    Task<InvoicePageResult> InvoicesGETAsync(string filter, string orderby, double? skip, double? top);
    Task<InvoicePageResult> InvoicesGETAsync(string filter, string orderby, double? skip, double? top, CancellationToken cancellationToken);
    Task<Invoice> InvoicesPOSTAsync(CreateInvoiceReq body);
    Task<Invoice> InvoicesPOSTAsync(CreateInvoiceReq body, CancellationToken cancellationToken);
    Task<AccountProfile> Profile2Async(string handle);
    Task<AccountProfile> Profile2Async(string handle, CancellationToken cancellationToken);
    Task<AccountProfile> ProfileAsync(Guid id);
    Task<AccountProfile> ProfileAsync(Guid id, CancellationToken cancellationToken);
    Task<InvoiceQuote> QuoteAsync(Guid invoiceId);
    Task<InvoiceQuote> QuoteAsync(Guid invoiceId, CancellationToken cancellationToken);
    Task<ICollection<Subscription>> SubscriptionsAllAsync();
    Task<ICollection<Subscription>> SubscriptionsAllAsync(CancellationToken cancellationToken);
    Task SubscriptionsDELETEAsync(Guid subscriptionId);
    Task SubscriptionsDELETEAsync(Guid subscriptionId, CancellationToken cancellationToken);
    Task<Subscription> SubscriptionsGETAsync(Guid subscriptionId);
    Task<Subscription> SubscriptionsGETAsync(Guid subscriptionId, CancellationToken cancellationToken);
    Task<Subscription> SubscriptionsPATCHAsync(Guid subscriptionId, UpdateSubscription body);
    Task<Subscription> SubscriptionsPATCHAsync(Guid subscriptionId, UpdateSubscription body, CancellationToken cancellationToken);
    Task<Subscription> SubscriptionsPOSTAsync(CreateSubscription body);
    Task<Subscription> SubscriptionsPOSTAsync(CreateSubscription body, CancellationToken cancellationToken);
    Task<ICollection<ConversionAmount>> TickerAsync();
    Task<ICollection<ConversionAmount>> TickerAsync(CancellationToken cancellationToken);
}
