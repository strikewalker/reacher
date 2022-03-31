using System.Net.Http;
using System.Net.Http.Headers;

namespace Reacher.Tests;
public class StrikeClientTests : TestBase
{
    private readonly StrikeClient _api;

    public StrikeClientTests()
    {
        var httpClient = new HttpClient { BaseAddress = new Uri("https://api.strike.me") };
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config["StrikeApiKey"]);
        _api = new StrikeClient(httpClient);
    }
    [Fact]
    public async Task TestGetUserProfile()
    {
        var profile = await _api.Profile2Async("bluesam");
        Assert.NotNull(profile);
    }
}
