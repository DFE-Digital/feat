namespace feat.ingestion.Services;

public interface IIngestionService
{
    Task IngestAsync(CancellationToken cancellationToken = default);
}