using feat.common.Models.AiSearch;

namespace feat.ingestion.Handlers;

public interface ISearchIndexHandler
{
    bool CreateIndex();

    Task<bool> Ingest(List<AiSearchEntry> entries, CancellationToken cancellationToken);

    IReadOnlyList<float> GetVector(string? text);
    
    Task Delete(IEnumerable<string> idsToDelete, CancellationToken cancellationToken);
}