using System.Text.Json.Serialization;
using feat.api;
using feat.api.Configuration;
using feat.api.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

builder.Services.Configure<AzureOptions>(
    builder.Configuration.GetSection("Azure"));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddSingleton<ISearchService, SearchService>();
builder.Services.AddSingleton<ApiClient>();

builder.Services.AddHttpClient(ExternalApi.Postcode, client =>
{
    client.BaseAddress = new Uri("https://api.postcodes.io/");
});

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();