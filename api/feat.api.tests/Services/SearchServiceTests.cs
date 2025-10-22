using Azure.Search.Documents;
using feat.api.Services;
using feat.common;
using feat.common.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using OpenAI.Embeddings;

namespace feat.api.tests.Services;

[TestFixture]
public class SearchServiceTests(
    IOptionsMonitor<AzureOptions> azureOptions,
    IApiClient apiClient,
    SearchClient searchClient,
    EmbeddingClient embeddingClient,
    SearchService searchService)
{
    private IOptionsMonitor<AzureOptions> _azureOptions = azureOptions;
    private IApiClient _apiClient = apiClient;
    private SearchClient _searchClient = searchClient;
    private EmbeddingClient _embeddingClient = embeddingClient;
    
    private SearchService _searchService = searchService;

    [SetUp]
    public void Setup()
    {
        _azureOptions = Substitute.For<IOptionsMonitor<AzureOptions>>();
        _apiClient = Substitute.For<IApiClient>();
        _searchClient = Substitute.For<SearchClient>();
        _embeddingClient = Substitute.For<EmbeddingClient>();
        
        _searchService = new SearchService(
            _azureOptions,
            _apiClient,
            _searchClient,
            _embeddingClient
        );
    }
}