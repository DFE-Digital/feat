using Azure.Search.Documents;
using feat.api.Data;
using feat.api.Services;
using feat.common;
using feat.common.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using OpenAI.Embeddings;
using ZiggyCreatures.Caching.Fusion;

namespace feat.api.tests.Services;

[TestFixture]
public class SearchServiceTests(
    IOptionsMonitor<AzureOptions> azureOptions,
    IApiClient apiClient,
    SearchClient searchClient,
    EmbeddingClient embeddingClient,
    CourseDbContext dbContext,
    SearchService searchService,
    IFusionCache cache)
{
    private IOptionsMonitor<AzureOptions> _azureOptions = azureOptions;
    private IApiClient _apiClient = apiClient;
    private SearchClient _searchClient = searchClient;
    private EmbeddingClient _embeddingClient = embeddingClient;
    private CourseDbContext _dbContext = dbContext;
    private IFusionCache _cache = cache;
    
    private SearchService _searchService = searchService;

    [SetUp]
    public void Setup()
    {
        _azureOptions = Substitute.For<IOptionsMonitor<AzureOptions>>();
        _apiClient = Substitute.For<IApiClient>();
        _searchClient = Substitute.For<SearchClient>();
        _embeddingClient = Substitute.For<EmbeddingClient>();
        _dbContext = Substitute.For<CourseDbContext>();
        _cache = Substitute.For<IFusionCache>();
        
        _searchService = new SearchService(
            _azureOptions,
            _apiClient,
            _searchClient,
            _embeddingClient,
            _dbContext,
            _cache
        );
    }
}