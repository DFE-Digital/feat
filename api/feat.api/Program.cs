using feat.api.Configuration;
using feat.api.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AzureOptions>(
    builder.Configuration.GetSection("Azure"));

builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddSingleton<ISearchService, SearchService>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.UseHttpsRedirection();
app.MapControllers();

app.Run();