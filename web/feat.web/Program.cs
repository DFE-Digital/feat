using System.Text.Json.Serialization;
using feat.common;
using feat.web.Configuration;
using feat.web.Services;
using GovUk.Frontend.AspNetCore;
using Microsoft.Extensions.Options;
using NetEscapades.AspNetCore.SecurityHeaders;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

// Add layered configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

if (builder.Environment.IsDevelopment() &&
    Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
{
    builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddEnvironmentVariables();

// Add configuration
builder.Services.Configure<SearchOptions>(builder.Configuration.GetSection(SearchOptions.Name));
builder.Services.Configure<AnalyticsOptions>(builder.Configuration.GetSection(AnalyticsOptions.Name));

// Add services to the container.
builder.Services.Configure<RouteOptions>(option =>
{
    option.LowercaseUrls = true;
    option.LowercaseQueryStrings = true;
});

builder.Services.AddGovUkFrontend(options =>
{
    options.Rebrand = true;
    options.FrontendPackageHostingOptions = FrontendPackageHostingOptions.None;
    options.GetCspNonceForRequest = context => context.GetNonce();
});

builder.Services.AddRazorPages().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddScoped<ISearchService, SearchService>();

builder.Services.AddHttpClient(ApiClientNames.Feat, (sp, client) =>
    {
        var options = sp.GetRequiredService<IOptions<SearchOptions>>().Value;
        client.BaseAddress = new Uri(options.ApiBaseUrl);
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        AllowAutoRedirect = true,
        UseDefaultCredentials = false
    });

// Setup our session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddScoped<StaticNavigationHandler>();

var app = builder.Build();

var policyCollection = new HeaderPolicyCollection()
    .AddDefaultSecurityHeaders()
    .AddContentSecurityPolicy(csp =>
    {
        csp.AddBlockAllMixedContent();
        csp.AddUpgradeInsecureRequests();
        csp.AddBaseUri()
            .Self();
        csp.AddDefaultSrc()
            .Self();
        csp.AddScriptSrc()
            .Self()
            .WithNonce()
            .From("*.googletagmanager.com")
            .From("*.google-analytics.com")
            .From("c.bing.com")
            .From("*.clarity.ms");
        csp.AddStyleSrc()
            .Self()
            .WithNonce()
            .From("rsms.me");
        csp.AddFontSrc()
            .Self()
            .From("res-1.cdn.office.net")
            .From("rsms.me");
        csp.AddConnectSrc()
            .Self()
            .From("*.googletagmanager.com")
            .From("*.google-analytics.com")
            .From("*.analytics.google.com")
            .From("c.bing.com")
            .From("*.clarity.ms");
    });
    
app.UseSecurityHeaders(policyCollection);

app.UseExceptionHandler("/Errors/500");
app.UseStatusCodePagesWithReExecute("/Errors/{0}");

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.UseStaticFiles();
app.MapStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();