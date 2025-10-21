using Azure.Search.Documents;
using feat.common.Configuration;
using feat.common.Models.AiSearch;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;

namespace feat.ingestion.Handlers;

public class SearchIndexHandler(
    IOptionsMonitor<AzureOptions> options,
    SearchClient aiSearchClient,
    EmbeddingClient embeddingClient)
    : ISearchIndexHandler
{
    private readonly AzureOptions _azureOptions = options.CurrentValue;
    private readonly SearchClient _aiSearchClient = aiSearchClient;
    private readonly EmbeddingClient _embeddingClient = embeddingClient;

    public bool CreateIndex()
    {
        
        throw new NotImplementedException();
    }

    public bool Ingest(SearchIndexingBufferedSender<AiSearchEntry> entries)
    {
        throw new NotImplementedException();
    }
}