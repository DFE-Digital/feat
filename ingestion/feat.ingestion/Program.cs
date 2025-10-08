using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
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

if (ingestionOptions.Environment.Equals("Development", StringComparison.InvariantCultureIgnoreCase))
{
    try
    {
        string connection = ingestionOptions.ConnectionString;
        
        using (var dbContext = new IngestionDbContext(connection))
        {
            var pendingMigrations = dbContext.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                Console.WriteLine("FEAT ingestion migrations found.");  
                dbContext.Database.Migrate();
                
                Console.WriteLine("FEAT ingestion migrations have been applied."); 
            }
            else
            {
                Console.WriteLine("FEAT ingestion no migrations found.");
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"FEAT ingestion development migration Exception: {e}");
    }
}

