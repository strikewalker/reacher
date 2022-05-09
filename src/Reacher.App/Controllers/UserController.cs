using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Reacher.App.Models;
using Reacher.Data;
using Reacher.Data.External;
using Reacher.Data.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Reacher.App.Controllers;
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly AppDbContext _db;
    private readonly ReacherSettings _settings;
    private readonly IStrikeFacade _strikeFacade;
    private readonly IEmailForwardingService _emailForwardingService;

    public UserController(ILogger<UserController> logger, AppDbContext db, IOptions<ReacherSettings> settings, IStrikeFacade strikeFacade, IEmailForwardingService emailForwardingService)
    {
        _logger = logger;
        _db = db;
        _settings = settings.Value;
        _strikeFacade = strikeFacade;
        _emailForwardingService = emailForwardingService;
    }

    private async Task<(Guid UserId, string Email, string StrikeUsername, string Name)> GetLoggedInUser()
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        var claims = jwtSecurityToken.Claims;
        var email = claims.First(c => c.Type == "email").Value;
        var username = claims.First(c => c.Type == "username").Value;
        var name = claims.First(c => c.Type == "name").Value;
        var id = Guid.Parse(claims.First(c => c.Type == "id").Value);
        return (UserId: id, Email: email, StrikeUsername: username, Name: name);
    }

    [HttpGet]
    public async Task<UserModel?> Get()
    {
        var user = await GetLoggedInUser();
        var dbUser = _db.Users.SingleOrDefault(u => u.Id == user.UserId);
        if (dbUser == null)
        {
            dbUser = new()
            {
                Id = user.UserId,
                EmailAddress = user.Email ?? "unknown",
                Name = user.Name ?? "unknown",
                StrikeUsername = user.StrikeUsername ?? "unknown",
            };
            _db.Users.Add(dbUser);
            await _db.SaveChangesAsync();
        }
        var reachable = await GetUserCurrentReachable(user.UserId);
        if (reachable == null)
        {
            reachable = await _db.Reachables.FirstOrDefaultAsync(r => r.StrikeUsername == user.StrikeUsername && r.UserId == null);
            if (reachable != null)
            {
                reachable.UserId = user.UserId;
                await _db.SaveChangesAsync();
            }
        }
        var unpaidEmails = await GetReachableRecentEmails(reachable.Id);
        var recentSenders = unpaidEmails.GroupBy(e => e.FromEmailAddress.ToUpperInvariant()).OrderByDescending(f => f.Count())
            .Select(g => new RecentSender { EmailAddress = g.First().FromEmailAddress, FromName = g.First().FromEmailName, MessageCount = g.Count() }).ToList();
        var whitelist = await GetUserWhitelist(user.UserId);
        var toReplace = _settings.IsTest ? "@testing.reacher.me" : "@reacher.me";
        var setupModel = new UserModel
        {
            User = new() { Name = user.Name, StrikeUsername = user.StrikeUsername },
            Config = new()
            {
                Name = reachable?.Name ?? user.Name,
                Price = reachable?.CostUsdToReach ?? .01m,
                ReacherEmailPrefix = reachable?.ReacherEmailAddress?.Replace(toReplace, ""),
                StrikeUsername = reachable?.StrikeUsername ?? user.StrikeUsername,
                DestinationEmail = reachable?.ToEmailAddress ?? user.Email,
                Currency = reachable?.Currency ?? (await GetCurrency(user.StrikeUsername)) ?? "USD",
                Disabled = reachable?.Disabled ?? false,
            },
            RecentSenders = recentSenders,
            Whitelist = new Whitelist { EmailAddresses = string.Join('\n', whitelist.OrderBy(o => o)) }
        };
        return setupModel;
    }

    [HttpPatch("whitelist")]
    public async Task UpdateWhitelist([FromBody] Whitelist request)
    {
        var user = await GetLoggedInUser();
        var whitelistItems = await GetUserWhitelistRecords(user.UserId);
        var emailAddresses = request.EmailAddresses.Split('\n', StringSplitOptions.RemoveEmptyEntries).Distinct().Select(e => new { EmailAddress = e.Trim(), Upper = e.Trim().ToUpperInvariant() }).ToList();
        var added = emailAddresses.Where(e => !whitelistItems.Any(w => w.EmailAddress.ToUpper() == e.Upper)).ToList();
        var removed = whitelistItems.Where(e => !emailAddresses.Any(w => w.Upper == e.EmailAddress.ToUpper()));

        _db.AddRange(added.ConvertAll(r => new DbWhitelist { UserId = user.UserId, EmailAddress = r.EmailAddress }));
        _db.RemoveRange(removed);
        await _db.SaveChangesAsync();

        var reachable = await GetUserCurrentReachable(user.UserId);
        if (reachable == null)
            return;

        var recentEmails = await GetReachableRecentEmails(user.UserId);
        foreach (var item in added)
        {
            var filtered = recentEmails.FindAll(r => r.FromEmailAddress.ToUpper() == item.Upper);
            foreach (var recentEmail in filtered)
            {
                await _emailForwardingService.ForwardEmail(recentEmail.Id);
            }
        }
    }

    private Task<List<string>> GetUserWhitelist(Guid userId)
    {
        return _db.Whitelist.Where(e => e.UserId == userId).Select(w => w.EmailAddress).ToListAsync();
    }
    private Task<List<DbWhitelist>> GetUserWhitelistRecords(Guid userId)
    {
        return _db.Whitelist.Where(e => e.UserId == userId).ToListAsync();
    }

    private Task<List<DbEmail>> GetReachableRecentEmails(Guid reachableId)
    {
        var dayAgo = DateTime.UtcNow.AddDays(-1);
        return _db.Emails.Where(e => e.CreatedDate > dayAgo && e.ReachableId == reachableId && e.Type == Data.Enums.EmailType.InboundReach && e.InvoiceStatus == Data.Enums.InvoiceStatus.Requested).ToListAsync();
    }

    private Task<DbReachable?> GetUserCurrentReachable(Guid userId)
    {
        return _db.Reachables.OrderByDescending(r => r.CreatedDate).FirstOrDefaultAsync(r => r.UserId == userId);
    }

    [HttpGet("currency")]
    public async Task<string?> GetCurrency(string strikeUsername)
    {
        try
        {
            var publicUser = await _strikeFacade.GetProfile(strikeUsername);
            return publicUser?.Currencies?.FirstOrDefault(c => c.IsDefaultCurrency)?.Currency.ToString();
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Unable to find Invoiceable currency for {User}", strikeUsername);
        }
        return null;
    }

    [HttpGet("isavailable")]
    public async Task<bool> GetIsAvailable(string prefix)
    {
        var user = await GetLoggedInUser();
        var otherHas = await _db.Reachables.AnyAsync(r => r.ReacherEmailAddress == getReacherEmail(prefix) && r.UserId != user.UserId);
        return !otherHas;
    }

    [HttpPost]
    public async Task Post([FromBody] SetupConfig config)
    {
        var user = await GetLoggedInUser();
        var reachable = await _db.Reachables.FirstOrDefaultAsync(r => r.UserId == user.UserId);
        if (reachable == null)
        {
            reachable = new()
            {
                UserId = user.UserId,
            };
            await _db.Reachables.AddAsync(reachable);
        }

        reachable.Name = config.Name;
        reachable.StrikeUsername = config.StrikeUsername;
        reachable.ReacherEmailAddress = getReacherEmail(config.ReacherEmailPrefix);
        reachable.ToEmailAddress = config.DestinationEmail;
        reachable.CostUsdToReach = config.Price;
        reachable.Currency = config.Currency;
        reachable.Disabled = config.Disabled;
        await _db.SaveChangesAsync();
    }

    private string getReacherEmail(string prefix)
    {
        return $"{prefix}@{(_settings.IsTest ? "testing." : "")}reacher.me";
    }
}
