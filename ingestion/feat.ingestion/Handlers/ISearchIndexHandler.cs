using feat.common.Models.AiSearch;

namespace feat.ingestion.Handlers;

public interface ISearchIndexHandler
{
    bool CreateIndex();

    Task<bool> Ingest(List<AiSearchEntry> entries);

    IReadOnlyList<float> GetVector(string? text);
}