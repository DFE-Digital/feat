using feat.ingestion.Handlers.DiscoverUni;
using feat.ingestion.Handlers.FAA;
using feat.ingestion.Handlers.FAC;
using feat.ingestion.Handlers.Geolocation;
using Microsoft.Extensions.DependencyInjection;

namespace feat.ingestion.Handlers;

public class IngestionHandlerFactory(IServiceProvider provider) : IIngestionHandlerFactory
{
    public IIngestionHandler? Create(string source) => source.ToUpperInvariant() switch
    {
        "FAC" => provider.GetRequiredService<FacIngestionHandler>(),
        "FAA" => provider.GetRequiredService<FaaIngestionHandler>(),
        "DU" or "DISCOVERUNI" => provider.GetRequiredService<DiscoverUniIngestionHandler>(),
        "GEOLOCATION" or "LOCATION" or "POSTCODE" => provider.GetRequiredService<GeolocationHandler>(),
        _ => null
    };
}