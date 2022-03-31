using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reacher.Data;

namespace Reacher.Tests;
public abstract class TestBase
{
    protected readonly IConfigurationRoot _config;
    private readonly ServiceProvider _provider;

    protected AppDbContext GetDbContext() => _provider.GetService<AppDbContext>();

    protected TestBase()
    {
        _config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.test.json")
           .AddUserSecrets<TestBase>()
           .AddEnvironmentVariables()
           .Build();

        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(opts =>
        {
            var connString = _config.GetConnectionString("AppDb");
            opts.UseSqlServer(connString);
        });
        _provider = services.BuildServiceProvider();
    }
}
