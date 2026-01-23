using System.Globalization;
using System.IO.Compression;
using Azure.Storage.Blobs;
using CsvHelper;
using feat.common.Extensions;
using feat.common.Models;
using feat.common.Models.AiSearch;
using feat.common.Models.Enums;
using feat.ingestion.Data;
using feat.ingestion.Enums;
using feat.ingestion.Models.Geolocation;
using feat.ingestion.Models.Geolocation.Converters;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Location = feat.common.Models.Location;

namespace feat.ingestion.Handlers.Geolocation;

public class GeolocationHandler(
    IngestionDbContext dbContext,
    ISearchIndexHandler searchIndexHandler,
    BlobServiceClient blobServiceClient) : IngestionHandler
{
    public override IngestionType IngestionType => IngestionType.Api;
    public override string Name => "Geolocation Handler";
    public override string Description => "Ingestion handler to grab postcode and location information from ONS";
#pragma warning disable CS0618 // Type or member is obsolete
    public override SourceSystem SourceSystem => SourceSystem.NotSpecified;
#pragma warning restore CS0618 // Type or member is obsolete

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
                        CleanName = (r.Name2Lang != null && r.Name2Lang.Equals("eng",
                                                            StringComparison.InvariantCultureIgnoreCase)
                                                        && r.Name2 != null
                            ? r.Name2.Replace("'", "").Replace("-", " ")
                            : r.Name1.Replace("'", "").Replace("-", " ")) + (!string.IsNullOrEmpty(r.County) ? $" {r.County}" : ""),
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

    public override async Task<bool> SyncAsync(CancellationToken cancellationToken)
    {
        List<Guid> entriesToUpdate = [];
        
        Console.WriteLine($"Starting sync for {Name}...");
        
        Console.WriteLine($"Fetching locations with missing geolocation that have a postcode...");
        
        // Use this to attempt to match up any locations that are incorrect
        var missingLocations = dbContext
            .Locations
            .Include(location => location.EntryInstances)
            .Where(l => l.GeoLocation == null && !string.IsNullOrWhiteSpace(l.Postcode) && l.EntryInstances.Any());

        // Loop through any matching postcodes and update the geolocation accordingly, plus pull out the list
        // of affected instance IDs
        Console.WriteLine($"Looking up geolocation for postcodes...");
        
        foreach (var missingLocation in missingLocations)
        {
            var lookupPostcode = missingLocation.Postcode?.Replace(" ", "");

            // Get the postcode from the location if we have one
            var postcode = await
                dbContext.LookupPostcodes.FirstOrDefaultAsync(p => 
                    p.CleanPostcode == lookupPostcode && p.Latitude >= -90 && p.Latitude <= 90 && p.Longitude >= -180 && p.Longitude <= 180, cancellationToken: cancellationToken);

            if (postcode is { Longitude: not null, Latitude: not null })
            {
                missingLocation.GeoLocation = postcode.ToPoint();
                missingLocation.Updated = DateTime.Now;
                entriesToUpdate.AddRange(missingLocation.EntryInstances.Select(ei => ei.EntryId));
            }
        }
        
        Console.WriteLine($"Fetching entries with a missing location but that have one in FAC...");
        // Match up any values where we're missing,but have it in the source
        var locationDifferences = from 
            ac in dbContext.FAC_AllCourses
            join ei in dbContext.EntryInstances on
                new {
                    SourceReference = ac.COURSE_RUN_ID.ToString(),
                    SourceSystem = (SourceSystem?)SourceSystem.FAC
                }
                equals new {
                    ei.SourceReference,
                    ei.SourceSystem
                } 
            where ac.LOCATION != null && (ei.Location == null || (ei.Location != null && ei.Location.GeoLocation == null))
        select new { AllCourse = ac, Instance = ei };

        Console.WriteLine($"Updating those locations...");
        var addedLocations = new List<Location>();
        foreach (var locationDifference in locationDifferences)
        {
            if (locationDifference.Instance.Location == null)
            {
                var newLocation = addedLocations.FirstOrDefault(l =>
                    l.Address1 == locationDifference.AllCourse.LOCATION_ADDRESS1 &&
                    l.Address2 == locationDifference.AllCourse.LOCATION_ADDRESS2 &&
                    l.Town == locationDifference.AllCourse.LOCATION_TOWN &&
                    l.County == locationDifference.AllCourse.LOCATION_COUNTY &&
                    l.Postcode == locationDifference.AllCourse.LOCATION_POSTCODE);

                if (newLocation != null)
                {
                    locationDifference.Instance.Location = newLocation;
                }
                else
                {
                    var location = new Location()
                    {
                        Created = DateTime.Now,
                        Address1 = locationDifference.AllCourse.LOCATION_ADDRESS1,
                        Address2 = locationDifference.AllCourse.LOCATION_ADDRESS2,
                        Town = locationDifference.AllCourse.LOCATION_TOWN,
                        County = locationDifference.AllCourse.LOCATION_COUNTY,
                        Postcode = locationDifference.AllCourse.LOCATION_POSTCODE,
                        Email = locationDifference.AllCourse.LOCATION_EMAIL,
                        Telephone = locationDifference.AllCourse.LOCATION_TELEPHONE,
                        SourceSystem = SourceSystem.FAC,
                        SourceReference = locationDifference.AllCourse.COURSE_RUN_ID.ToString(),
                        Url = locationDifference.AllCourse.LOCATION_WEBSITE,
                        GeoLocation = locationDifference.AllCourse.LOCATION
                    };
                    addedLocations.Add(location);
                    locationDifference.Instance.Location = location;
                }

                entriesToUpdate.Add(locationDifference.Instance.EntryId);
            }
            else if (locationDifference.Instance.Location.Postcode != null && 
                     locationDifference.AllCourse.LOCATION_POSTCODE != null && 
                     locationDifference.Instance.Location.Postcode.Replace(" ", "").Trim() == locationDifference.AllCourse.LOCATION_POSTCODE.Replace(" ", "").Trim())
            {
                locationDifference.Instance.Location.GeoLocation = locationDifference.AllCourse.LOCATION;
                locationDifference.Instance.Location.Updated = DateTime.Now;
                entriesToUpdate.Add(locationDifference.Instance.EntryId);
            }
        }

        Console.WriteLine($"Creating {addedLocations.Count} locations...");
        
        var entries = entriesToUpdate.Distinct().ToList();

        var locationsToUpdate =
            dbContext.Entries.WhereBulkContains(entries).SelectMany(e => e.EntryInstances)
                .Include(ei => ei.Location)
                .Include(ei => ei.Entry);

        Console.WriteLine($"Updating {locationsToUpdate.Count()} locations...");
        
        foreach (var entryInstance in locationsToUpdate)
        {
            var aiSearchEntry = dbContext.AiSearchEntries.Single(aie => aie.InstanceId == entryInstance.Id.ToString());
            aiSearchEntry.Location = entryInstance.Location?.GeoLocation.ToGeographyPoint();
            entryInstance.Entry.IngestionState = IngestionState.ProcessingGeolocation;
        }
        
        Console.WriteLine($"Finding AI Search locations with different locations to the DB...");
        
        // Grab our differences
        var tolerance = 0.000000001;
        var aiDifferences = 
            from aiEntry in dbContext.AiSearchEntries
            join ei in dbContext.EntryInstances on 
                aiEntry.InstanceId equals ei.Id.ToString()
            where 
                aiEntry.LocationPoint != null && ei.Location != null && ei.Location.GeoLocation != null &&
                (
                    Math.Abs(aiEntry.LocationPoint.X - ei.Location.GeoLocation.X) > tolerance || 
                    Math.Abs(aiEntry.LocationPoint.X - ei.Location.GeoLocation.X) > tolerance
                )
            select new { aiEntry, ei.Location, ei.Entry };

        // Set them to processing
        Console.WriteLine($"Updating AI Entries for {aiDifferences.Count()} entries...");
        
        foreach (var locationDifference in aiDifferences)
        {
            locationDifference.aiEntry.LocationPoint = locationDifference.Location.GeoLocation;
            locationDifference.Entry.IngestionState = IngestionState.ProcessingGeolocation;
        }
        
        Console.WriteLine($"Writing changes to DB...");
        await dbContext.BulkSaveChangesAsync(cancellationToken);
        
        Console.WriteLine($"Done");
        return true;
    }

    public override async Task<bool> IndexAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Starting indexing for {Name}...");
        
        var aiEntries = from aiEntry in dbContext.AiSearchEntries
            join ei in dbContext.EntryInstances on aiEntry.InstanceId equals ei.Id.ToString()
            join e in dbContext.Entries on ei.EntryId equals e.Id
            where e.IngestionState == IngestionState.ProcessingGeolocation
            select aiEntry;
        
        Console.WriteLine($"Updating search index for {aiEntries.Count()} entries...");
        
        await searchIndexHandler.Update(aiEntries.ToList(), cancellationToken);

        Console.WriteLine($"Marking entries as indexed...");
        
        await dbContext.Entries.WhereBulkContains(aiEntries.Select(aie => Guid.Parse(aie.Id)).ToList())
            .ExecuteUpdateAsync(x => x.SetProperty(e => e.IngestionState, IngestionState.Complete), cancellationToken);
        
        Console.WriteLine($"Done");
        
        return true;
    }
}