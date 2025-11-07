using feat.ingestion.Enums;

namespace feat.ingestion.Handlers;

public interface IIngestionHandler
{
    public IngestionType IngestionType { get; }
    
    public string Name { get; }
    
    public string Description { get; }

    public bool Ingest();

    public bool Validate();
    
    public bool Sync();
    
    public Task<bool> IngestAsync(CancellationToken cancellationToken);

    public Task<bool> ValidateAsync(CancellationToken cancellationToken);
    
    public Task<bool> Sync(CancellationToken cancellationToken);
}