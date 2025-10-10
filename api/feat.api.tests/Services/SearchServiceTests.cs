using Azure.Search.Documents;
using feat.api.Configuration;
using feat.api.Services;
using feat.common;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using OpenAI.Embeddings;

namespace feat.api.tests.Services;

[TestFixture]
public class SearchServiceTests
{
    private IOptionsMonitor<AzureOptions> _azureOptions;
    private IApiClient _apiClient;
    private SearchClient _searchClient;
    private EmbeddingClient _embeddingClient;
    
    private SearchService _searchService;
    
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