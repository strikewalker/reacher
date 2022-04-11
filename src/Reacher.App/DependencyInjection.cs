using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Reacher.App.Services;
using Reacher.Common.Utilities;
using Reacher.Data;
using Reacher.Data.External;
using Reacher.Data.External.Strike;
using SendGrid;
using System.Net.Http.Headers;

namespace Reacher.App;
public static class DependencyInjection
{
    public static void AddDependencies(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureClients(builder =>
        {
            builder.AddBlobServiceClient(configuration.GetConnectionString("AzureStorage"));
        });
        services.AddScoped<IEmailContentService, EmailContentService>();
        services.Configure<ReacherSettings>(configuration.GetSection("ReacherSettings"));
        services.AddDbContext<AppDbContext>(opts =>
        {
            var connString = configuration.GetConnectionString("AppDb");
            opts.UseSqlServer(connString, options =>
            {
                options.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName.Split(',')[0]);
            });
        });
        services.AddHttpClient<IStrikeClient, StrikeClient>((x, client) =>
        {
            var settings = x.GetService<IOptions<ReacherSettings>>().Value;
            client.BaseAddress = new Uri(settings.StrikeApiUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.StrikeApiKey);
        });
        services.AddScoped<ITemplateService, TemplateService>();
        services.AddScoped<IEmailContentRenderer, EmailContentRenderer>();
        services.AddScoped<IEmailIngestionService, EmailIngestionService>();
        services.AddSingleton<ISendGridFacade, SendGridFacade>();
        services.AddSingleton<IStrikeFacade, StrikeFacade>();
        services.AddSingleton<ISendGridParser, SendGridParser>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddSingleton<ISendGridClient>(x =>
        {
            var settings = x.GetService<IOptions<ReacherSettings>>().Value;
            return new SendGridClient(new() { ApiKey = settings.SendGridApiKey });
        });


        services.AddControllersWithViews().AddNewtonsoftJson();
    }
}
