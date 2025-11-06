using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure;
using Azure.AI.OpenAI;
using Azure.Core.Serialization;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using feat.common;
using feat.common.Configuration;
using feat.ingestion.Configuration;
using feat.ingestion.Data;
using feat.ingestion.Handlers;
using feat.ingestion.Handlers.FAA;
using feat.ingestion.Handlers.FAC;
using Microsoft.EntityFrameworkCore;
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

var cachingOptions = new CacheOptions();
config.GetSection(CacheOptions.Name).Get<CacheOptions>();

if (string.IsNullOrEmpty(ingestionOptions.Environment))
{
    Console.WriteLine("Environment missing in configuration.");
    return;
}

if (string.IsNullOrEmpty(ingestionOptions.ConnectionString))
{
    Console.WriteLine("ConnectionString missing in configuration.");
    return;
}

var services = new ServiceCollection();

services.AddDbContext<IngestionDbContext>(options =>
{
    options.UseSqlServer(ingestionOptions.ConnectionString, o => o.UseNetTopologySuite());
});

services.AddTransient<IMigrationsHandler, MigrationsHandler>();
services.AddTransient<ISearchIndexHandler, SearchIndexHandler>();
services.AddTransient<FacIngestionHandler>();
services.AddTransient<FaaIngestionHandler>();
services.AddSingleton<IApiClient, ApiClient>();
services.AddSingleton<IIngestionHandlerFactory, IngestionHandlerFactory>();
services.AddSingleton(ingestionOptions);
services.Configure<AzureOptions>(config.GetSection(AzureOptions.Name));

switch (cachingOptions?.Type)
{
    case "Memory":
        services.AddDistributedMemoryCache();
        services.AddFusionCacheMemoryBackplane();
        services.AddFusionCache()
            .WithRegisteredSerializer()
            .WithSerializer(
                new FusionCacheSystemTextJsonSerializer()
            )
            .WithDefaultEntryOptions(new FusionCacheEntryOptions()
            {
                Duration = cachingOptions?.Duration ?? TimeSpan.FromDays(30),
                SkipBackplaneNotifications = true
            });
        break;
    case "Redis":
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = cachingOptions.ConnectionString;
        });
        services.AddFusionCacheStackExchangeRedisBackplane(options =>
        {
            options.Configuration = cachingOptions.ConnectionString;
        });
        
        services.AddFusionCache()
            .WithOptions(opt =>
            {
                opt.CacheKeyPrefix = "";
                opt.DistributedCacheCircuitBreakerDuration = TimeSpan.FromSeconds(2);
            })
            .WithRegisteredSerializer()
            .WithRegisteredDistributedCache()
            .WithStackExchangeRedisBackplane()
            .WithSerializer(
                new FusionCacheSystemTextJsonSerializer()
            )
            .WithDefaultEntryOptions(new FusionCacheEntryOptions()
            {
                Duration = cachingOptions?.Duration ?? TimeSpan.FromDays(30),
                DistributedCacheDuration = cachingOptions?.Duration ?? TimeSpan.FromDays(30),
                
                IsFailSafeEnabled = true,
                FailSafeMaxDuration = TimeSpan.FromHours(2),
                FailSafeThrottleDuration = TimeSpan.FromSeconds(30),

                EagerRefreshThreshold = 0.9f,

                FactorySoftTimeout = TimeSpan.FromMilliseconds(100),
                FactoryHardTimeout = TimeSpan.FromMilliseconds(1500),

                DistributedCacheSoftTimeout = TimeSpan.FromSeconds(1),
                DistributedCacheHardTimeout = TimeSpan.FromSeconds(2),
                AllowBackgroundDistributedCacheOperations = true,

                // JITTERING
                JitterMaxDuration = TimeSpan.FromSeconds(2)
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
}

Console.WriteLine("FEAT ingestion service end.");