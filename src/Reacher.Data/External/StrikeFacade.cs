using Reacher.Data.External.Strike;
using Reacher.Data.External.Strike.Models;

namespace Reacher.Data.External;

public class StrikeFacade : IStrikeFacade
{
    private readonly IStrikeClient _strikeClient;

    public StrikeFacade(IStrikeClient strikeClient)
    {
        _strikeClient = strikeClient;
    }

    public async Task<Guid> CreateInvoice(string strikeUsername, decimal amountUSD, string description, Guid emailId)
    {
        var result = await _strikeClient.HandleAsync(strikeUsername, new()
        {
            Amount = new Strike.Models.CurrencyAmount
            {
                Amount = amountUSD.ToString("f2"),
                Currency = Strike.Models.CurrencyAmountCurrency.USD
            },
            Description = description,
            CorrelationId = emailId.ToString()
        });
        return result.InvoiceId;
    }

    public async Task<bool> InvoiceIsPaid(Guid strikeInvoiceId)
    {
        var invoice = await _strikeClient.InvoicesGET2Async(strikeInvoiceId);
        return invoice.State == Strike.Models.InvoiceState.PAID;
    }

    public Task<InvoiceQuote> CreateLnInvoice(Guid strikeInvoiceId)
    {
        return _strikeClient.QuoteAsync(strikeInvoiceId);
    }
}

public interface IStrikeFacade
{
    Task<Guid> CreateInvoice(string strikeUsername, decimal amountUSD, string description, Guid emailId);
    Task<InvoiceQuote> CreateLnInvoice(Guid strikeInvoiceId);
    Task<bool> InvoiceIsPaid(Guid strikeInvoiceId);
}