using feat.common;
using feat.common.Configuration;
using feat.ingestion.Handlers;
using feat.ingestion.Handlers.DiscoverUni;
using feat.ingestion.Handlers.FAA;
using feat.ingestion.Handlers.FAC;
using feat.ingestion.Handlers.Geolocation;
using Microsoft.Extensions.DependencyInjection;

namespace feat.ingestion.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddIngestionServices()
        {
            // Add our search index handlers
            services.AddTransient<ISearchIndexHandler, SearchIndexHandler>();
            
            // Add any HTTP clients
            services.AddSingleton<IApiClient, ApiClient>();
            
            // Register individual handlers
            services.AddTransient<FaaIngestionHandler>();
            services.AddTransient<FacIngestionHandler>();
            services.AddTransient<DiscoverUniIngestionHandler>();
            services.AddTransient<GeolocationHandler>();
        
            // Register the ingestion handler factory
            services.AddSingleton<IIngestionHandlerFactory, IngestionHandlerFactory>();

            return services; // Return the IServiceCollection for method chaining
        }
    }
}