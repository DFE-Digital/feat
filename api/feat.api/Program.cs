using Microsoft.EntityFrameworkCore;
using feat.api.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure;
using Azure.AI.OpenAI;
using Azure.Core.Serialization;
using Azure.Search.Documents;
using feat.api.Configuration;
using feat.api.Services;
using feat.common;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;
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

var connectionString = builder.Configuration.GetConnectionString("IngestionConnection");
builder.Services.AddDbContext<IngestionDbContext>(options =>
    options.UseSqlServer(connectionString, x => x.UseNetTopologySuite()));

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
builder.Services.AddSingleton<IApiClient, ApiClient>();

builder.Services.AddHttpClient(ApiClientNames.Postcode, client =>
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