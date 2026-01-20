using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure;
using Azure.AI.OpenAI;
using Azure.Core.Serialization;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Storage.Blobs;
using feat.common;
using feat.common.Configuration;
using feat.ingestion.Configuration;
using feat.ingestion.Data;
using feat.ingestion.Extensions;
using feat.ingestion.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

Console.WriteLine("FEAT ingestion service started.");

var currentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{currentEnvironment}.json", false)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
    .AddEnvironmentVariables();

var config = builder.Build();

var ingestionOptions = new IngestionOptions();
config.GetSection(IngestionOptions.Name).Bind(ingestionOptions);

var cacheOptions = new CacheOptions();
config.GetSection(CacheOptions.Name).Bind(cacheOptions);

if (string.IsNullOrEmpty(ingestionOptions.Environment))
{
    Console.WriteLine("Environment missing in configuration.");
    return;
}

var ingestionConnectionString = config.GetConnectionString("Ingestion");
if (string.IsNullOrEmpty(ingestionConnectionString))
{
    Console.WriteLine("ConnectionString Ingestion missing in configuration.");
    return;
}

var blobStorageConnectionString = config.GetConnectionString("BlobStorage");
if (string.IsNullOrEmpty(config.GetConnectionString("BlobStorage")))
{
    Console.WriteLine("Blob storage connection string not set");
    return;
}

var services = new ServiceCollection();

services.AddDbContext<IngestionDbContext>(options =>
{
    options.UseSqlServer(ingestionConnectionString, o => o.UseNetTopologySuite());
});

services.AddTransient<IMigrationsHandler, MigrationsHandler>();
services.Configure<AzureOptions>(config.GetSection(AzureOptions.Name));
services.Configure<CacheOptions>(config.GetSection(CacheOptions.Name));
services.AddSingleton(ingestionOptions);

services.AddIngestionServices();

switch (cacheOptions.Type)
{
    case CacheOptions.Memory:
        services.AddMemoryCache();
        services.AddFusionCache()
            .WithSerializer(
                new FusionCacheSystemTextJsonSerializer()
            )
            .WithDefaultEntryOptions(new FusionCacheEntryOptions()
            {
                Duration = cacheOptions?.Duration ?? TimeSpan.FromMinutes(5),
                SkipBackplaneNotifications = true
            });
        break;
    case CacheOptions.Redis:
        var cacheConnectionString = config.GetConnectionString("Cache");
        if (string.IsNullOrEmpty(cacheConnectionString))
        {
            Console.WriteLine("ConnectionString Cache missing in configuration.");
            return;
        }
        
        services.AddFusionCache()
            .WithDistributedCache(_ =>
            {
                var options = new RedisCacheOptions { Configuration = cacheConnectionString };
                return new RedisCache(options);
            })
            .WithStackExchangeRedisBackplane(x => x.Configuration = cacheConnectionString )
            .WithSerializer(
                new FusionCacheSystemTextJsonSerializer()
            )
            .WithDefaultEntryOptions(new FusionCacheEntryOptions()
            {
                Duration = cacheOptions?.Duration ?? TimeSpan.FromMinutes(5),
                DistributedCacheDuration = cacheOptions?.L2Duration ?? TimeSpan.FromDays(30)
            });
        break;
    default:
        services.AddFusionCache()
            .WithoutDistributedCache()
            .WithoutBackplane()
            .WithSerializer(
                new FusionCacheSystemTextJsonSerializer()
            )
            .WithDefaultEntryOptions(new FusionCacheEntryOptions()
            {
                Duration = TimeSpan.Zero
            });
        break;
}

services.AddOpenTelemetry()
    // SETUP TRACES
    .WithTracing(tracing => tracing
            .AddFusionCacheInstrumentation()
            .AddConsoleExporter() // OR ANY ANOTHER EXPORTER
    )
    // SETUP METRICS
    .WithMetrics(metrics => metrics
            .AddFusionCacheInstrumentation()
            .AddConsoleExporter() // OR ANY ANOTHER EXPORTER
    );

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

services.AddSingleton<SearchIndexClient>(sp =>
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
    
    return new SearchIndexClient(
        new Uri(options.AiSearchUrl),
        new AzureKeyCredential(options.AiSearchAdminKey),
        clientOptions);
});


services.AddSingleton<BlobServiceClient>(_ => new BlobServiceClient(blobStorageConnectionString));

services.AddSingleton<EmbeddingClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<AzureOptions>>().Value;
    
    var openAiClient = new AzureOpenAIClient(
        new Uri(options.OpenAiEndpoint), new AzureKeyCredential(options.OpenAiKey));
    
    return openAiClient.GetEmbeddingClient("text-embedding-3-large");
});

services.AddHttpClient(ApiClientNames.FindAnApprenticeship, client =>
{
    client.BaseAddress = new Uri("https://api.apprenticeships.education.gov.uk/");
    
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ingestionOptions.ApprenticeshipApiKey);
    client.DefaultRequestHeaders.Add("X-Version", "2");
    client.DefaultRequestHeaders.Add("AdditionalDataSources", "Nhs");
});

var serviceProvider = services.BuildServiceProvider();

if (ingestionOptions.Environment.Equals("Development", StringComparison.InvariantCultureIgnoreCase))
{
    try
    {
        var migrationsHandler = serviceProvider.GetService<IMigrationsHandler>();
        
        if (migrationsHandler?.HasPendingModelChanges() ?? false)
        {
            Console.WriteLine(
                "There are model changes that have not been applied as migrations, these need to be created first");
            return;
        }
        
        migrationsHandler?.RunPendingMigrations();
        
        var searchIndexHandler = serviceProvider.GetService<ISearchIndexHandler>();
        searchIndexHandler?.CreateIndex();
    }
    catch (Exception e)
    {
        Console.WriteLine($"FEAT ingestion development migration Exception: {e}");
    }
}

var factory = serviceProvider.GetRequiredService<IIngestionHandlerFactory>();

foreach (var argument in args)
{
    var handler = factory.Create(argument);
    if (handler == null)
    {
        Console.WriteLine($"Unknown ingestion source: {argument}");
        continue;
    }

    Console.WriteLine($"Valid: {handler.Validate()}");
    Console.WriteLine($"Ingest: {handler.Ingest()}");
    Console.WriteLine($"Sync: {handler.Sync()}");
    Console.WriteLine($"Index: {handler.Index()}");
}

Console.WriteLine("FEAT ingestion service end.");