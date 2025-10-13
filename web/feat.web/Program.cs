using System.Text.Json.Serialization;
using feat.web.Configuration;
using feat.web.Repositories;
using feat.web.Services;
using GovUk.Frontend.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Services.Configure<SearchOptions>(builder.Configuration.GetSection(SearchOptions.Search));

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
});
builder.Services.AddRazorPages().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});


// Setup our HTTP client
builder.Services.AddHttpClient("httpClient")
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler()
        {
            AllowAutoRedirect = true,
            UseDefaultCredentials = false
        };
    });
builder.Services.AddSingleton<HttpClientRepository>();
builder.Services.AddSingleton<ISearchService, SearchService>();

// Setup our session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8081);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.UseStaticFiles();
app.MapStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();