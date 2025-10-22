using feat.common.Configuration;
using feat.ingestion.Configuration;
using feat.ingestion.Enums;
using Microsoft.Extensions.Options;

namespace feat.ingestion.Handlers;

public class IngestionHandler(IngestionOptions options) : IIngestionHandler
{
    public virtual IngestionType IngestionType => IngestionType.Manual;
    public virtual string Name => "Default Ingestion Handler";
    public virtual string Description => "This should not be used and should be inherited as the base class";

    public virtual bool Ingest()
    {
        var task = IngestAsync(CancellationToken.None);
        task.Wait();
        return task.Result;
    }

    public virtual bool Validate()
    {
        var task = ValidateAsync(CancellationToken.None);
        task.Wait();
        return task.Result;
    }

    public virtual Task<bool> IngestAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual Task<bool> ValidateAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}