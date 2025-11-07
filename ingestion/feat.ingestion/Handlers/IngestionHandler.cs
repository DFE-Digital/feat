using feat.ingestion.Configuration;
using feat.ingestion.Enums;

namespace feat.ingestion.Handlers;

public abstract class IngestionHandler(IngestionOptions options) : IIngestionHandler
{
    public virtual IngestionType IngestionType => IngestionType.Manual;
    public virtual string Name => "Default Ingestion Handler";
    public virtual string Description => "This should not be used and should be inherited as the base class";
    
    public bool Ingest()
    {
        return IngestAsync(CancellationToken.None).GetAwaiter().GetResult();
    }
    
    public bool Validate()
    {
        return ValidateAsync(CancellationToken.None).GetAwaiter().GetResult();
    }
    
    public bool Sync()
    {
        return Sync(CancellationToken.None).GetAwaiter().GetResult();
    }
    
    public abstract Task<bool> IngestAsync(CancellationToken cancellationToken);
    
    public abstract Task<bool> ValidateAsync(CancellationToken cancellationToken);
    
    public abstract Task<bool> Sync(CancellationToken cancellationToken);
}