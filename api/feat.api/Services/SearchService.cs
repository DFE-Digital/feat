using System.Text.Json;
using System.Text.Json.Serialization;
using Azure;
using Azure.AI.OpenAI;
using Azure.Core.Serialization;
using Azure.Search.Documents;
using feat.api.Configuration;
using feat.api.Models;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;

namespace feat.api.Services;

public class SearchService : ISearchService
{
    private readonly AzureOptions _azureOptions;
    private readonly AzureOpenAIClient _openAiClient;
    private readonly SearchClient _aiSearchClient;
    private readonly EmbeddingClient _embeddingClient;
    
    public SearchService(IOptionsMonitor<AzureOptions> options)
    {
        _azureOptions = options.CurrentValue;
        
        var serializerOptions = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter(),
            },
        };
        
        // Search client
        var searchEndpoint = new Uri(_azureOptions.AiSearchUrl);
        var searchCredential = new AzureKeyCredential(_azureOptions.AiSearchKey);
        var searchClientOptions = new SearchClientOptions
        {
            Serializer = new JsonObjectSerializer(serializerOptions),
            Diagnostics =
            {
                IsLoggingContentEnabled = true,
                IsTelemetryEnabled = true,
            },
        };
        _aiSearchClient = new SearchClient(
            searchEndpoint, _azureOptions.AiSearchIndex, searchCredential, searchClientOptions);
            
        // OpenAI client
        // var embeddingsEndpoint = new Uri(_azureOptions.OpenAiEndpoint);
        // var openAiCredential = new AzureKeyCredential(_azureOptions.OpenAiKey);
        // _openAiClient = new AzureOpenAIClient(embeddingsEndpoint, openAiCredential);
        // _embeddingClient = _openAiClient.GetEmbeddingClient("text-embedding-3-large");
    }
    
    public async Task<SearchResponse?> SearchAsync(SearchRequest request)
    {
        //var embedding = await _embeddingClient.GenerateEmbeddingAsync(request.Query);
        
        var search = await _aiSearchClient.SearchAsync<AiSearchCourse>(request.Query);

        var searchResults = search.Value;
        
        var result = new SearchResponse
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = searchResults.TotalCount,
        };

        return result;
    }
}