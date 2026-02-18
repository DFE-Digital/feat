using Microsoft.EntityFrameworkCore;
using feat.api.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure;
using Azure.AI.OpenAI;
using Azure.Core.Serialization;
using Azure.Search.Documents;
using feat.api.Services;
using feat.common;
using feat.common.Configuration;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AzureOptions>(
    builder.Configuration.GetSection(AzureOptions.Name));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true;
    //options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.Converters.Add(new MicrosoftSpatialGeoJsonConverter());
});

var connectionString = builder.Configuration.GetConnectionString("Courses");
builder.Services.AddDbContext<CourseDbContext>(options =>
    options.UseSqlServer(connectionString, x => x.UseNetTopologySuite()));

builder.Services.Configure<CacheOptions>(
    builder.Configuration.GetSection(CacheOptions.Name));

var cacheOptions = new CacheOptions();
builder.Configuration.GetSection(CacheOptions.Name).Bind(cacheOptions);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddSingleton<SearchClient>(sp =>
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

builder.Services.AddSingleton<EmbeddingClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<AzureOptions>>().Value;
    
    var openAiClient = new AzureOpenAIClient(
        new Uri(options.OpenAiEndpoint), new AzureKeyCredential(options.OpenAiKey));
    
    return openAiClient.GetEmbeddingClient("text-embedding-3-large");
});

builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddSingleton<IApiClient, ApiClient>();

builder.Services.AddHttpClient(ApiClientNames.Postcode, client =>
{
    client.BaseAddress = new Uri("https://api.postcodes.io/");
});

switch (cacheOptions?.Type)
{
    case "Memory":
        builder.Services.AddMemoryCache();
        builder.Services.AddFusionCache()
            .WithSerializer(
                new FusionCacheSystemTextJsonSerializer()
            )
            .WithDefaultEntryOptions(new FusionCacheEntryOptions()
            {
                Duration = cacheOptions?.Duration ?? TimeSpan.FromMinutes(5),
                SkipBackplaneNotifications = true
            });
        break;
    case "Redis":
        var cacheConnectionString = builder.Configuration.GetConnectionString("Cache");
        if (string.IsNullOrEmpty(cacheConnectionString))
        {
            Console.WriteLine("ConnectionString Cache missing in configuration.");
            return;
        }
        
        builder.Services.AddFusionCache()
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
                Duration = cacheOptions?.Duration ?? TimeSpan.FromHours(1),
                DistributedCacheDuration = cacheOptions?.L2Duration ?? TimeSpan.FromDays(3)
            });
        break;
    default:
        builder.Services.AddFusionCache()
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

builder.Services.AddOpenTelemetry()
    // SETUP TRACES
    .WithTracing(tracing => tracing
            .AddFusionCacheInstrumentation()
    )
    // SETUP METRICS
    .WithMetrics(metrics => metrics
            .AddFusionCacheInstrumentation()
    );

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();
var policyCollection = new HeaderPolicyCollection()
    .AddDefaultApiSecurityHeaders()
    .AddDefaultSecurityHeaders();

app.UseSecurityHeaders(policyCollection);

app.MapMcp("/mcp");
app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();
app.UseHttpsRedirection();

await app.RunAsync();