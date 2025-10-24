using feat.ingestion.Enums;

namespace feat.ingestion.Handlers;

public interface IIngestionHandler
{
    public IngestionType IngestionType { get; }
    
    public string Name { get; }
    
    public string Description { get; }

    public bool Ingest();

    public bool Validate();
    
    public Task<bool> IngestAsync(CancellationToken cancellationToken);

    public Task<bool> ValidateAsync(CancellationToken cancellationToken);
}