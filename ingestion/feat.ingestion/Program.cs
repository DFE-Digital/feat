using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using feat.ingestion;
using feat.ingestion.Data;


Console.WriteLine("FEAT ingestion service started.");

var configBuilder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
    .AddEnvironmentVariables();

var config = configBuilder.Build();

var ingestionOptions = new IngestionOptions();
config.GetSection("IngestionOptions").Bind(ingestionOptions);

if (string.IsNullOrEmpty(ingestionOptions.Environment) ||
    string.IsNullOrEmpty(ingestionOptions.ConnectionString))
{
    Console.WriteLine("FEAT Environment variable missing.");
    return;
}

string connectionString = ingestionOptions.ConnectionString;


// Set up DI
var services = new ServiceCollection();
services.AddDbContext<IngestionDbContext>(options =>
    options.UseSqlServer(connectionString, optionsBuilder => optionsBuilder.UseNetTopologySuite()));
services.AddTransient<IMigrationsHandler, MigrationsHandler>();

ServiceProvider serviceProvider = services.BuildServiceProvider();

if (ingestionOptions.Environment.Equals("Development", StringComparison.InvariantCultureIgnoreCase))
{
    try
    {        
        var migrationsHandler = serviceProvider.GetService<IMigrationsHandler>();
        migrationsHandler?.RunPendingMigrations();
    }
    catch (Exception e)
    {
        Console.WriteLine($"FEAT ingestion development migration Exception: {e}");
    }
}


Console.WriteLine("FEAT ingestion service end.");
