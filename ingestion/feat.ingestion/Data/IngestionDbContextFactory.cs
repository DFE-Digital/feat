using System.Reflection;
using feat.ingestion.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace feat.ingestion.Data;

public class IngestionDbContextFactory : IDesignTimeDbContextFactory<IngestionDbContext>
{
    public IngestionDbContext CreateDbContext(string[] args)
    {
        var currentEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (string.IsNullOrEmpty(currentEnvironment))
        {
            currentEnvironment = "Development";
        }
        
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{currentEnvironment}.json", false)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
            .AddEnvironmentVariables();

        var config = builder.Build();

        var ingestionOptions = new IngestionOptions();
        config.GetSection(IngestionOptions.Name).Bind(ingestionOptions);

        string connectionString = ingestionOptions.ConnectionString;
        
        var optionsBuilder = new DbContextOptionsBuilder<IngestionDbContext>();
        optionsBuilder.UseSqlServer(connectionString, b => b
            .UseNetTopologySuite()
        );

        return new IngestionDbContext(optionsBuilder.Options);
    }
}