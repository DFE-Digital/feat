using Azure.Search.Documents;
using feat.common.Models.AiSearch;

namespace feat.ingestion.Data;

public interface IIndexHandler
{
    bool CreateIndex();

    bool Ingest(SearchIndexingBufferedSender<AiSearchEntry> entries);
}