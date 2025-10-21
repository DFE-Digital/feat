using Azure.Search.Documents;
using feat.common.Models.AiSearch;

namespace feat.ingestion.Handlers;

public interface ISearchIndexHandler
{
    bool CreateIndex();

    bool Ingest(SearchIndexingBufferedSender<AiSearchEntry> entries);
}