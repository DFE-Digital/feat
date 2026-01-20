using feat.common.Models.Enums;
using feat.ingestion.Enums;

namespace feat.ingestion.Handlers;

public abstract class IngestionHandler : IIngestionHandler
{
    public virtual IngestionType IngestionType => IngestionType.Manual;
    public virtual string Name => "Default Ingestion Handler";
    public virtual string Description => "This should not be used and should be inherited as the base class";
#pragma warning disable CS0618 // Type or member is obsolete
    public virtual SourceSystem SourceSystem => SourceSystem.NotSpecified;
#pragma warning restore CS0618 // Type or member is obsolete

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
        return SyncAsync(CancellationToken.None).GetAwaiter().GetResult();
    }
    
    public bool Index()
    {
        return IndexAsync(CancellationToken.None).GetAwaiter().GetResult();
    }
    
    public abstract Task<bool> IngestAsync(CancellationToken cancellationToken);
    
    public abstract Task<bool> ValidateAsync(CancellationToken cancellationToken);
    
    public abstract Task<bool> SyncAsync(CancellationToken cancellationToken);
    
    public abstract Task<bool> IndexAsync(CancellationToken cancellationToken);
}