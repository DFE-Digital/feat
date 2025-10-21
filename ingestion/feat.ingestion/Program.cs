using feat.common;
using feat.ingestion.Configuration;
using feat.ingestion.Data;
using feat.ingestion.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

var configBuilder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly(), optional: true)
    .AddEnvironmentVariables();

var config = configBuilder.Build();

var ingestionOptions = new IngestionOptions();
config.GetSection("IngestionOptions").Bind(ingestionOptions);

if (string.IsNullOrEmpty(ingestionOptions.ConnectionString))
{
    Console.WriteLine("ConnectionString missing in configuration.");
    return;
}

if (string.IsNullOrEmpty(ingestionOptions.ApprenticeshipApiKey))
{
    Console.WriteLine("ApprenticeshipApiKey missing in configuration.");
    return;
}

var services = new ServiceCollection();
services.AddLogging();

services.AddDbContext<IngestionDbContext>(options =>
{
    options.UseSqlServer(ingestionOptions.ConnectionString, o => o.UseNetTopologySuite());
});

services.AddTransient<IMigrationsHandler, MigrationsHandler>();

services.AddHttpClient(ApiClientNames.FindAnApprenticeship, client =>
{
    client.BaseAddress = new Uri("https://api.apprenticeships.education.gov.uk/");
    
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ingestionOptions.ApprenticeshipApiKey);
    client.DefaultRequestHeaders.Add("X-Version", "2");
    client.DefaultRequestHeaders.Add("AdditionalDataSources", "Nhs");
});

services.AddSingleton<IApiClient, ApiClient>();
services.AddTransient<IIngestionService, FindAnApprenticeshipIngestionService>();

await using var serviceProvider = services.BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Program");

try
{
    if (environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
    {
        var migrationsHandler = serviceProvider.GetService<IMigrationsHandler>();
        migrationsHandler?.RunPendingMigrations();
    }

    logger.LogInformation("Starting ingestion services...");

    var ingestionServices = serviceProvider.GetServices<IIngestionService>();
    var tasks = ingestionServices.Select(s => s.IngestAsync());
    await Task.WhenAll(tasks);

    logger.LogInformation("All ingestion services completed successfully.");
}
catch (Exception ex)
{
    logger.LogError(ex, "Error running ingestion services.");
}
