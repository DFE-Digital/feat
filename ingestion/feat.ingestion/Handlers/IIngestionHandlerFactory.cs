namespace feat.ingestion.Handlers;

public interface IIngestionHandlerFactory
{
    IIngestionHandler? Create(string source);
}