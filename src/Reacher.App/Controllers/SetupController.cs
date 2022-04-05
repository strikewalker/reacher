using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Reacher.App.Models;
using Reacher.Data;
using Reacher.Data.External;
using System.IdentityModel.Tokens.Jwt;

namespace Reacher.App.Controllers;
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SetupController : ControllerBase
{
    private readonly ILogger<SetupController> _logger;
    private readonly AppDbContext _db;
    private readonly ReacherSettings _settings;
    private readonly IStrikeFacade _strikeFacade;

    public SetupController(ILogger<SetupController> logger, AppDbContext db, IOptions<ReacherSettings> settings, IStrikeFacade strikeFacade)
    {
        _logger = logger;
        _db = db;
        _settings = settings.Value;
        _strikeFacade = strikeFacade;
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
    public async Task<SetupModel?> Get()
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
        var reachable = await _db.Reachables.FirstOrDefaultAsync(r => r.UserId == user.UserId);
        if (reachable == null)
        {
            reachable = await _db.Reachables.FirstOrDefaultAsync(r => r.StrikeUsername == user.StrikeUsername && r.UserId == null);
            if (reachable != null)
            {
                reachable.UserId = user.UserId;
                await _db.SaveChangesAsync();
            }
        }
        var toReplace = _settings.IsTest ? "@testing.reacher.me" : "@reacher.me";
        var setupModel = new SetupModel
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
            }
        };
        return setupModel;
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
