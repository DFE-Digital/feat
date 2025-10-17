using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure;
using Azure.AI.OpenAI;
using Azure.Core.Serialization;
using Azure.Search.Documents;
using feat.common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using feat.ingestion;
using feat.ingestion.Configuration;
using feat.ingestion.Data;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;


Console.WriteLine("FEAT ingestion service started.");

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

if (string.IsNullOrEmpty(ingestionOptions.Environment))
{
    Console.WriteLine("FEAT Environment name missing.");
    return;
}
if (string.IsNullOrEmpty(ingestionOptions.ConnectionString))
{
    Console.WriteLine("FEAT connection string missing.");
    return;
}

string connectionString = ingestionOptions.ConnectionString;


// Set up DI
var services = new ServiceCollection();
services.AddDbContext<IngestionDbContext>(options =>
    options.UseSqlServer(connectionString, optionsBuilder => optionsBuilder
        .UseNetTopologySuite()
        .MigrationsAssembly("feat.common")
    ));
services.AddTransient<IMigrationsHandler, MigrationsHandler>();

services.AddSingleton<SearchClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<AzureOptions>>().Value;
    
    var serializerOptions = new JsonSerializerOptions
    {
        Converters =
        {
            new JsonStringEnumConverter(),
            new MicrosoftSpatialGeoJsonConverter()
        },
    };
    
    var clientOptions = new SearchClientOptions
    {
        Serializer = new JsonObjectSerializer(serializerOptions)
    };
    
    return new SearchClient(
        new Uri(options.AiSearchUrl),
        options.AiSearchIndex,
        new AzureKeyCredential(options.AiSearchKey),
        clientOptions);
});

services.AddSingleton<EmbeddingClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<AzureOptions>>().Value;
    
    var openAiClient = new AzureOpenAIClient(
        new Uri(options.OpenAiEndpoint), new AzureKeyCredential(options.OpenAiKey));
    
    return openAiClient.GetEmbeddingClient("text-embedding-3-large");
});



ServiceProvider serviceProvider = services.BuildServiceProvider();

if (ingestionOptions.Environment.Equals("Development", StringComparison.InvariantCultureIgnoreCase))
{
    try
    {
        var migrationsHandler = serviceProvider.GetService<IMigrationsHandler>();
        
        if (migrationsHandler?.HasPendingModelChanges() ??  false)
        {
            Console.WriteLine(
                "There are model changes that have not been applied as migrations, these need to be created first");
            return;
        }
        
        migrationsHandler?.RunPendingMigrations();
        
        
    }
    catch (Exception e)
    {
        Console.WriteLine($"FEAT ingestion development migration Exception: {e}");
    }
}


Console.WriteLine("FEAT ingestion service end.");
