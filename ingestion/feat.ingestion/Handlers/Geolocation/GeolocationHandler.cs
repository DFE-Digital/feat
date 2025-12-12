using System.Globalization;
using Azure.Storage.Blobs;
using CsvHelper;
using feat.common.Models;
using feat.common.Models.Enums;
using feat.ingestion.Configuration;
using feat.ingestion.Data;
using feat.ingestion.Enums;
using feat.ingestion.Models.FAC;
using feat.ingestion.Models.Geolocation;

namespace feat.ingestion.Handlers.Geolocation;

public class GeolocationHandler(
    IngestionOptions options,
    IngestionDbContext dbContext,
    BlobServiceClient blobServiceClient) : IngestionHandler(options)
{
    public override IngestionType IngestionType => IngestionType.Api;
    public override string Name => "Geolocation Handler";
    public override string Description => "Ingestion handler to grab postcode and location information from ONS";
    public override SourceSystem SourceSystem => SourceSystem.NotSpecified;

    private const string ContainerName = "geolocation";


    public override async Task<bool> IngestAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Starting ingestion for {Name}...");
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var valid = await ValidateAsync(cancellationToken);

        // If we're not passing validation, stop
        if (!valid)
        {
            Console.WriteLine($"Unable to validate {Name} data, stopping");
            return false;
        }

        // Let's stream our data in
        var files = containerClient.GetBlobsAsync(cancellationToken: cancellationToken)
            .ToBlockingEnumerable(cancellationToken).ToArray();

        // Get our latest postcode Data file
        var postcodeData = files.Where(blob =>
                blob.Name.StartsWith("postcode_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).FirstOrDefault();

        if (postcodeData != null)
        {
            Console.WriteLine("Starting import of postcode data");
            var blobClient = containerClient.GetBlobClient(postcodeData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<PostcodeLatLongMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<PostcodeLatLong>().ToList();


            Console.WriteLine($"Preparing {records.Count} records to DB...");
            await dbContext.BulkSynchronizeAsync(records, options => { options.UseTableLock = true; },
                cancellationToken);


            Console.WriteLine("Done");
        }

        // Get our latest postcode Data file
        var locationData = files.Where(blob =>
                blob.Name.StartsWith("england_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).FirstOrDefault();

        if (locationData != null)
        {
            Console.WriteLine("Starting import of location data");
            var blobClient = containerClient.GetBlobClient(locationData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<LocationLatLongMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<LocationLatLong>().ToList();


            Console.WriteLine($"Preparing {records.Count} records to DB...");
            await dbContext.BulkSynchronizeAsync(records, options => { options.UseTableLock = true; },
                cancellationToken);


            Console.WriteLine("Done");
        }

        return true;

    }

    public override async Task<bool> ValidateAsync(CancellationToken cancellationToken)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        var exists = await containerClient.ExistsAsync(cancellationToken);
        if (!exists)
        {
            Console.WriteLine("Storage container doesn't exist, attempting to create");
        }

        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        exists = await containerClient.ExistsAsync(cancellationToken);

        // Check the container exists
        if (!exists)
        {
            Console.WriteLine($"Unable create the {Name} Azure Storage Container");
            return false;
        }

        // Now check we have some files
        var files = containerClient.GetBlobsAsync(cancellationToken: cancellationToken);

        var foundPostcodes = false;
        var foundLocations = false;

        await foreach (var blob in files)
        {
            if (blob.Name.StartsWith("postcode_", StringComparison.InvariantCultureIgnoreCase))
            {
                foundPostcodes = true;
            }

            if (blob.Name.StartsWith("england_", StringComparison.InvariantCultureIgnoreCase))
            {
                foundLocations = true;
            }
            
        }

        if (!foundPostcodes)
        {
            Console.WriteLine("Unable to find postcodes data file");
            return false;
        }

        if (!foundLocations)
        {
            Console.WriteLine("Unable to find locations data file");
            return false;
        }

        // Otherwise, we're returning true
        return true;
    }

    public override Task<bool> SyncAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    public override Task<bool> IndexAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }
}