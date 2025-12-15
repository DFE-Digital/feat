using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using Azure.Storage.Blobs;
using CsvHelper;
using feat.common.Models;
using feat.common.Models.Enums;
using feat.ingestion.Configuration;
using feat.ingestion.Data;
using feat.ingestion.Enums;
using feat.ingestion.Models.FAC;
using feat.ingestion.Models.Geolocation;
using feat.ingestion.Models.Geolocation.Converters;

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
                blob.Name.StartsWith("ONSPD_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).FirstOrDefault();

        if (postcodeData != null)
        {
            Console.WriteLine("Starting import of postcode data");
            var blobClient = containerClient.GetBlobClient(postcodeData.Name);
            Console.WriteLine("Fetching file");
            var stream = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
            await using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

            List<PostcodeLatLong> postcodes = [];
            
            foreach (var postcodeFile in archive.Entries
                         .Where(e => e.FullName.StartsWith("Data/multi_csv") 
                                     && e.Name.EndsWith(".csv")))
            {
                using var reader = new StreamReader(await postcodeFile.OpenAsync(cancellationToken));
                Console.WriteLine($"Setting up CSV reader for {postcodeFile.Name}");
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Context.RegisterClassMap<PostcodeMap>();
                Console.WriteLine("Reading data...");
                var records = csv.GetRecordsAsync<Postcode>(cancellationToken);
                
                postcodes.AddRange(records
                    .Where(p => p.CountryCode != null && p.CountryCode.StartsWith('E'))
                    .Select(p => new PostcodeLatLong()
                    {
                        Postcode = p.Code,
                        CleanPostcode = p.Code.Replace(" ", ""),
                        Latitude = p.Latitude,
                        Longitude = p.Longitude,
                        Expired = p.Expired
                    }).ToBlockingEnumerable(cancellationToken: cancellationToken));
            }

            Console.WriteLine($"Preparing {postcodes.Count} records to DB...");
            await dbContext.BulkSynchronizeAsync(postcodes, options => { options.UseTableLock = true; },
                cancellationToken);

            Console.WriteLine("Done");
        }
        
        // Get our latest postcode Data file
        var onsLocationData = files.Where(blob =>
                blob.Name.StartsWith("OPNAME_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).FirstOrDefault();

        if (onsLocationData != null)
        {
            Console.WriteLine("Starting import of ONS Name data");
            var blobClient = containerClient.GetBlobClient(onsLocationData.Name);
            Console.WriteLine("Fetching file");
            var stream = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
            await using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

            List<LocationLatLong> locations = [];
            
            foreach (var locationFile in archive.Entries
                         .Where(e => e.FullName.StartsWith("Data") 
                                     && e.Name.EndsWith(".csv")))
            {
                using var reader = new StreamReader(await locationFile.OpenAsync(cancellationToken));
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Context.RegisterClassMap<OpenNameMap>();
                var records = csv.GetRecordsAsync<OpenName>(cancellationToken);

                locations.AddRange(records
                    .Where(r => r is
                    {
                        Country: "England", 
                        Type: "populatedPlace",
                        LocalType: "City" or "Town" or "Village" or "Hamlet"
                    })
                    .Select(r => new LocationLatLong()
                    {
                        Name = (r.Name2Lang != null && r.Name2Lang.Equals("eng",
                                                        StringComparison.InvariantCultureIgnoreCase)
                                                    && r.Name2 != null
                            ? r.Name2
                            : r.Name1) + (!string.IsNullOrEmpty(r.County) ? $" ({r.County})" : ""),
                        CleanName = r.Name2Lang != null && r.Name2Lang.Equals("eng",
                                                            StringComparison.InvariantCultureIgnoreCase)
                                                        && r.Name2 != null
                            ? r.Name2.Replace("'", "").Replace("-", " ")
                            : r.Name1.Replace("'", "").Replace("-", " "),
                        County = r.County,
                        Latitude = EastingsNorthingsLatLong.ToLatLong(r.X, r.Y).Latitude,
                        Longitude = EastingsNorthingsLatLong.ToLatLong(r.X, r.Y).Longitude
                    }).ToBlockingEnumerable(cancellationToken: cancellationToken));
            }

            locations = locations.DistinctBy(l => new { l.Name, l.County }).ToList();

            Console.WriteLine($"Preparing {locations.Count} records to DB...");
            await dbContext.BulkSynchronizeAsync(locations, options => { options.UseTableLock = true; },
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