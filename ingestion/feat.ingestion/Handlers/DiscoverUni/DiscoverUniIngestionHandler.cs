using System.Globalization;
using System.IO.Compression;
using Azure.Storage.Blobs;
using CsvHelper;
using feat.common.Extensions;
using feat.common.Models;
using feat.common.Models.AiSearch;
using feat.common.Models.Enums;
using feat.ingestion.Configuration;
using feat.ingestion.Data;
using feat.ingestion.Enums;
using feat.ingestion.Models.DU;
using feat.ingestion.Models.DU.Enums;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Z.BulkOperations;
using IngestionState = feat.common.Models.Enums.IngestionState;
using DU = feat.ingestion.Models.DU;
using Location = feat.common.Models.Location;

namespace feat.ingestion.Handlers.DiscoverUni;

public class DiscoverUniIngestionHandler(
    IngestionOptions options,
    IngestionDbContext dbContext,
    ISearchIndexHandler searchIndexHandler,
    BlobServiceClient blobServiceClient)
    : IngestionHandler(options)
{
    public override IngestionType IngestionType => IngestionType.Csv | IngestionType.Manual;
    public override string Name => "Discover Uni";
    public override string Description => "File based ingestion from Discover Uni dataset downloaded from the website";

    private const string ContainerName = "discoveruni";


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

        return true;

    }

    public override async Task<bool> IngestAsync(CancellationToken cancellationToken)
    {
        bool changes = false;
        var Download = ProcessMode.Skip;
        var Extract = ProcessMode.Skip;
        var Aims = ProcessMode.Force;
        var Locations = ProcessMode.Force;
        var Providers = ProcessMode.Force;
        var HECOS = ProcessMode.Force;
        var Courses = ProcessMode.Force;
        
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var valid = await ValidateAsync(cancellationToken);

        const string discoverUniDownload =
            "https://unistatsdatasetdownloadfunction.azurewebsites.net/api/UnistatsDatasetDownload";

        // Let's determine when we last fetched data
        Models.DU.IngestionState ingestionState;

        if (!dbContext.DU_IngestionState.Any())
        {
            ingestionState = new Models.DU.IngestionState()
            {
                Id = Guid.NewGuid(),
                DownloadComplete = false,
                Extracted = false,
                ETag = null
            };

            dbContext.DU_IngestionState.Add(ingestionState);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        ingestionState = dbContext.DU_IngestionState.First();




        var tempPath = Path.Combine(Path.GetTempPath(), "discoveruni.zip");
        
        // We need to download the DiscoverUni zip file
        using var http = new HttpClient();
        using var response = await http.GetAsync(discoverUniDownload, cancellationToken);
        if (response is { IsSuccessStatusCode: true, Headers.ETag.Tag: not null })
        {
            var etag = response.Headers.ETag.Tag;

            // If we have no etag or the etag is different, download
            // If the download failed or was incomplete, download
            if ((!etag.Equals(ingestionState.ETag) ||
                !ingestionState.DownloadComplete ||
                Download == ProcessMode.Force) && Download != ProcessMode.Skip)
            {
                ingestionState.ETag = etag;
                ingestionState.DownloadComplete = false;
                ingestionState.Extracted = false;
                await dbContext.SaveChangesAsync(cancellationToken);

                // Our etags are different, so we need to download this file
                Console.WriteLine($"Downloading latest {Name} data...");

                await using var downloadStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                await using var filestream = new FileStream(tempPath, FileMode.Create, FileAccess.Write);
                await downloadStream.CopyToAsync(filestream, cancellationToken);
                await filestream.FlushAsync(cancellationToken);
                filestream.Close();

                Console.WriteLine($"Done");
                
                ingestionState.DownloadComplete = true;

                await dbContext.SaveChangesAsync(cancellationToken);

                changes = true;

            }
        }

        if ((!ingestionState.Extracted || Extract == ProcessMode.Force) && File.Exists(tempPath) && Extract != ProcessMode.Skip)
        {
            // We need to extract the files we need
            
            using var archive = ZipFile.OpenRead(tempPath);
            foreach (var entry in archive.Entries)
            {
                if (entry.FullName.EndsWith(Path.GetFileName("INSTITUTION.csv")))
                {
                    Console.WriteLine("Uploading providers...");
                    await containerClient.DeleteBlobIfExistsAsync("INSTITUTION.csv", cancellationToken: cancellationToken);
                    await containerClient.UploadBlobAsync("INSTITUTION.csv", entry.Open(), cancellationToken);
                    Console.WriteLine("Done.");
                }

                if (entry.FullName.EndsWith(Path.GetFileName("LOCATION.csv")))
                {
                    Console.WriteLine("Uploading locations...");
                    await containerClient.DeleteBlobIfExistsAsync("LOCATION.csv", cancellationToken: cancellationToken);
                    await containerClient.UploadBlobAsync("LOCATION.csv", entry.Open(), cancellationToken);
                    Console.WriteLine("Done.");
                }

                if (entry.FullName.EndsWith(Path.GetFileName("KISCOURSE.csv")))
                {
                    Console.WriteLine("Uploading courses...");
                    await containerClient.DeleteBlobIfExistsAsync("KISCOURSE.csv", cancellationToken: cancellationToken);
                    await containerClient.UploadBlobAsync("KISCOURSE.csv", entry.Open(), cancellationToken);
                    Console.WriteLine("Done.");
                }

                if (entry.FullName.EndsWith(Path.GetFileName("COURSELOCATION.csv")))
                {
                    Console.WriteLine("Uploading course locations...");
                    await containerClient.DeleteBlobIfExistsAsync("COURSELOCATION.csv", cancellationToken: cancellationToken);
                    await containerClient.UploadBlobAsync("COURSELOCATION.csv", entry.Open(), cancellationToken);
                    Console.WriteLine("Done.");
                }
                
                if (entry.FullName.EndsWith(Path.GetFileName("KISAIM.csv")))
                {
                    Console.WriteLine("Uploading AIM Codes...");
                    await containerClient.DeleteBlobIfExistsAsync("KISAIM.csv", cancellationToken: cancellationToken);
                    await containerClient.UploadBlobAsync("KISAIM.csv", entry.Open(), cancellationToken);
                    Console.WriteLine("Done.");
                }
            }

            ingestionState.Extracted = true;
            await dbContext.SaveChangesAsync(cancellationToken);
            changes = true;
        }


        const int batchSize = 5000;
        // If we're not passing validation, stop
        if (!valid)
        {
            Console.WriteLine($"Unable to validate {Name}, stopping");
            return false;
        }

        if (!changes && 
            Courses != ProcessMode.Force && 
            Providers != ProcessMode.Force && 
            Locations != ProcessMode.Force)
        {
            Console.WriteLine($"No changes detected for {Name}, stopping");
            return true;
        }

        // Let's stream our data in
        var files = containerClient.GetBlobsAsync(cancellationToken: cancellationToken)
            .ToBlockingEnumerable(cancellationToken).ToArray();

        // Get our latest aim levels file
        var aimData = files.Where(blob =>
                blob.Name.StartsWith("KISAIM", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (aimData != null && Aims != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of aim level data...");
            var blobClient = containerClient.GetBlobClient(aimData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<DU.AimMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<DU.Aim>().ToList();

            // Determine if there are any changes
            if (records.Count != 0)
            {

                if (
                    Aims == ProcessMode.Force
                    || dbContext.DU_Aims.Count() != records.Count
                )
                {
                    Console.WriteLine($"Preparing {records.Count} records to DB...");
                    await dbContext.BulkSynchronizeAsync(records, options =>
                    {
                        options.BatchSize = batchSize;
                        options.BatchDelayInterval = 1000;
                        options.UseTableLock = true;
                    }, cancellationToken);
                }
            }

            Console.WriteLine("Done");
        }
        
        // Get our latest HECOS sectors file
        var hecosData = files.Where(blob =>
                blob.Name.StartsWith("HECOS", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (hecosData != null && HECOS != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of HECOS sector data...");
            var blobClient = containerClient.GetBlobClient(hecosData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<DU.HecosMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<DU.Hecos>().ToList();

            // Determine if there are any changes
            if (records.Count != 0)
            {

                if (
                    HECOS == ProcessMode.Force
                    || dbContext.DU_Aims.Count() != records.Count
                )
                {
                    Console.WriteLine($"Preparing {records.Count} records to DB...");
                    await dbContext.BulkSynchronizeAsync(records, options =>
                    {
                        options.BatchSize = batchSize;
                        options.BatchDelayInterval = 1000;
                        options.UseTableLock = true;
                    }, cancellationToken);
                }
            }

            Console.WriteLine("Done");
        }

        // Get our latest locations file
        var locationData = files.Where(blob =>
                blob.Name.StartsWith("LOCATION", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (locationData != null && Locations != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of location data...");
            var blobClient = containerClient.GetBlobClient(locationData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<DU.LocationMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<DU.Location>().ToList();

            // Determine if there are any changes
            if (records.Count != 0)
            {

                if (
                    Locations == ProcessMode.Force
                    || dbContext.DU_Locations.Count() != records.Count
                )
                {
                    Console.WriteLine($"Preparing {records.Count} records to DB...");
                    await dbContext.BulkSynchronizeAsync(records, options =>
                    {
                        options.BatchSize = batchSize;
                        options.BatchDelayInterval = 1000;
                        options.UseTableLock = true;
                    }, cancellationToken);
                }
            }

            Console.WriteLine("Done");
        }
        
        // Get our latest institutions file
        var institutionData = files.Where(blob =>
                blob.Name.StartsWith("INSTITUTION", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (institutionData != null && Providers != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of institution data...");
            var blobClient = containerClient.GetBlobClient(institutionData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<InstitutionMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<Institution>().Distinct().ToList();

            // Determine if there are any changes
            if (records.Count != 0)
            {

                if (
                    Providers == ProcessMode.Force
                    || dbContext.DU_Institutions.Count() != records.Count
                )
                {
                    Console.WriteLine($"Preparing {records.Count} records to DB...");
                    await dbContext.BulkSynchronizeAsync(records, options =>
                    {
                        options.BatchSize = batchSize;
                        options.BatchDelayInterval = 1000;
                        options.UseTableLock = true;
                    }, cancellationToken);
                }
            }

            Console.WriteLine("Done");
        }
        
        // Get our latest courses file
        var courseData = files.Where(blob =>
                blob.Name.StartsWith("KISCOURSE", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (courseData != null && Courses != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of course data...");
            var blobClient = containerClient.GetBlobClient(courseData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<DU.CourseMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<DU.Course>().ToList();

            // Determine if there are any changes
            if (records.Count != 0)
            {

                if (
                    Courses == ProcessMode.Force
                    || dbContext.DU_Courses.Count() != records.Count
                )
                {
                    Console.WriteLine($"Preparing {records.Count} records to DB...");
                    await dbContext.BulkSynchronizeAsync(records, options =>
                    {
                        options.BatchSize = batchSize;
                        options.BatchDelayInterval = 1000;
                        options.UseTableLock = true;
                    }, cancellationToken);
                }
            }

            Console.WriteLine("Done");
        }
        
        // Get our latest course locations file
        var courseLocationData = files.Where(blob =>
                blob.Name.StartsWith("COURSELOCATION", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (courseLocationData != null && Courses != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of course location data...");
            var blobClient = containerClient.GetBlobClient(courseLocationData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<DU.CourseLocationMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<DU.CourseLocation>().ToList();

            // Determine if there are any changes
            if (records.Count != 0)
            {

                if (
                    Courses == ProcessMode.Force
                    || dbContext.DU_CourseLocations.Count() != records.Count
                )
                {
                    Console.WriteLine($"Preparing {records.Count} records to DB...");
                    await dbContext.BulkSynchronizeAsync(records, options =>
                    {
                        options.BatchSize = batchSize;
                        options.BatchDelayInterval = 1000;
                        options.UseTableLock = true;
                    }, cancellationToken);
                }
            }

            Console.WriteLine("Done");
        }
        
        Console.WriteLine($"{Name} Ingestion Done");

        return true;
    }

    public override async Task<bool> SyncAsync(CancellationToken cancellationToken)
    {
        var resultInfo = new ResultInfo();
        var auditEntries = new List<AuditEntry>();
        bool skip = false;

        Console.WriteLine($"Starting sync of {Name} data");

        if (skip)
        {
            Console.WriteLine("Skipped");
            return true;
        }

        // LOCATIONS

        Console.WriteLine("Generating locations...");
        var locations =
            from l in dbContext.DU_Locations
            where l.Country == RegionCode.XF
            select new Location
            {
                Created = DateTime.Now,
                Updated = DateTime.Now,
                Name = l.Name,
                GeoLocation = new Point(new Coordinate(l.Longitude, l.Latitude)) { SRID = 4326 },
                Url = l.StudentUnionUrl,
                SourceReference = $"{l.UKPRN}_{l.LocationId}",
                SourceSystem = SourceSystem.DiscoverUni
            };

       

        await dbContext.BulkSynchronizeAsync(locations.Distinct(), options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = l => new
            {
                l.Id,
                l.Created,
                l.Updated
            };
            options.ColumnPrimaryKeyExpression = l => l.SourceReference;
            options.ColumnSynchronizeDeleteKeySubsetExpression = l => l.SourceSystem;
            options.UseRowsAffected = true;
            options.ResultInfo = resultInfo;
        }, cancellationToken);
        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();

        // INSTITUTIONS
        Console.WriteLine("Generating providers...");
        var providers =
            from i in dbContext.DU_Institutions
            select new Provider()
            {
                SourceSystem = SourceSystem.DiscoverUni,
                SourceReference = $"{i.UKPRN}_{i.PubUKPRN}",
                Created = DateTime.Now,
                Updated = DateTime.Now,
                Name = i.Name,
                Ukprn = i.UKPRN.ToString(),
                Pubukprn = i.PubUKPRN.ToString(),
                TradingName = i.TradingName,
                OtherNames = i.OtherNames
            };

        await dbContext.BulkSynchronizeAsync(providers, options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = p => new
            {
                p.Id,
                p.Created,
                p.Updated
            };
            options.ColumnPrimaryKeyExpression = l => new { l.Ukprn, l.Pubukprn };
            options.ColumnSynchronizeDeleteKeySubsetExpression = l => l.SourceSystem;
            options.UseRowsAffected = true;
            options.ResultInfo = resultInfo;
        }, cancellationToken);
        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();

        // PROVIDER LOCATIONS
        Console.WriteLine("Generating provider locations...");
        var providerLocations =
            from i in dbContext.DU_Institutions
            join p in dbContext.Providers
                on Convert.ToString(i.UKPRN) + "_" + Convert.ToString(i.PubUKPRN) equals p.SourceReference
            join ul in dbContext.DU_Locations
                on i.UKPRN equals ul.UKPRN
            join l in dbContext.Locations
                on Convert.ToString(ul.UKPRN) + "_" + ul.LocationId equals l.SourceReference
            where l.SourceSystem == SourceSystem.DiscoverUni
            select new ProviderLocation()
            {
                ProviderId = p.Id,
                LocationId = l.Id,
                SourceSystem = SourceSystem.DiscoverUni
            };

        var providerLocationList = providerLocations.ToList();
        
        await dbContext.BulkSynchronizeAsync(providerLocationList.Distinct(), options =>
        {
            options.ColumnPrimaryKeyExpression = p => new
            {
                p.ProviderId, p.LocationId
            };
            options.UseRowsAffected = true;
            options.ColumnSynchronizeDeleteKeySubsetExpression = e => e.SourceSystem;
            options.ResultInfo = resultInfo;
        }, cancellationToken);
        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();

        // ENTRY
        var courses = 
            from c in dbContext.DU_Courses
            join p in dbContext.Providers on
                new { UKPRN = c.UKPRN.ToString(), PubUKPRN = c.PubUKPRN.ToString() } 
                equals new { UKPRN = p.Ukprn, PubUKPRN = p.Pubukprn }
            join a in dbContext.DU_Aims on
                c.Aim equals a.AimCode into aims
            from a in aims.DefaultIfEmpty()
            
            
            select new Entry()
            {
                Created = DateTime.Now,
                Updated = DateTime.Now,
                SourceReference = $"{c.UKPRN}_{c.CourseId}_{c.StudyMode}",
                SourceSystem = SourceSystem.DiscoverUni,
                SourceUpdated = DateTime.Now,
                Type = EntryType.UniversityCourse,
                ProviderId = p.Id,

                Title = a != null ? 
                    $"{a.Label}{(c.Honours.GetValueOrDefault(false) ? " (Hons)" : "")} {c.Title}" : 
                    c.Honours.GetValueOrDefault(false) ? $"(Hons) {c.Title}" : c.Title,
                
                AimOrAltTitle = string.Empty,
                Description = string.Empty,
                EntryRequirements = string.Empty,
                WhatYouWillLearn = string.Empty,
                Url = c.CourseUrl ?? string.Empty,
                FlexibleStart = false,
                Reference = c.CourseId,
                AttendancePattern = c.StudyMode.ToCourseHours(),
                CourseType = CourseType.Degree
            };

        Console.WriteLine($"Generating entries for {courses.LongCount()} courses...");
        var distinctEntries = courses.Distinct();

        // ENTRY - Merged List

        await dbContext.BulkSynchronizeAsync(distinctEntries, options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = p => new
            {
                p.Id,
                p.Created,
                p.Updated,
                p.SourceUpdated,
                p.IngestionState
            };
            options.UseRowsAffected = true;
            options.ColumnPrimaryKeyExpression = e => e.SourceReference;
            options.ColumnSynchronizeDeleteKeySubsetExpression = e => e.SourceSystem;
            options.UseAudit = false;
            options.AuditEntries = auditEntries;
            options.ResultInfo = resultInfo;
        }, cancellationToken);
        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();
        // Run through the audit entries and check to see which entries were created or updated
        var createdIds = auditEntries.Where(e => e.Action == AuditActionType.Insert)
            .SelectMany(e => e.Values.Where(ae => ae.ColumnName == "Id").Select(ae => (Guid)ae.NewValue));

        // For all of our created entries, we'll need to set those to be indexed
        var createdEntries = dbContext.Entries.WhereBulkContains(createdIds);
        Console.WriteLine($"Setting ingestion status for {createdEntries.Count()} created entries...");
        await createdEntries.ForEachAsync(e => e.IngestionState = IngestionState.Pending,
            cancellationToken: cancellationToken);

        // We're only interested here if any text fields have changed
        var updatedIds = auditEntries.Where(e => e.Action == AuditActionType.Update
                                                 && e.Values.Exists(ae =>
                                                     ae.ColumnName is "Title" or "AimOrAltTitle" or "Description" &&
                                                     !Equals(ae.OldValue, ae.NewValue)))
            .SelectMany(e => e.Values.Where(ae => ae.ColumnName == "Id").Select(ae => (Guid)ae.NewValue));

        var updatedEntries = dbContext.Entries.WhereBulkContains(updatedIds);
        Console.WriteLine($"Setting ingestion status for {updatedEntries.Count()} updated entries...");
        await updatedEntries.ForEachAsync(e => e.IngestionState = IngestionState.Pending,
            cancellationToken: cancellationToken);
        await dbContext.BulkSaveChangesAsync(cancellationToken);
        Console.WriteLine("Setting ingestion status done.");

        var courseRuns =
            from c in dbContext.DU_Courses
            join e in dbContext.Entries on
                Convert.ToString(c.UKPRN) + "_" + c.CourseId + "_" + Convert.ToString((int)c.StudyMode)
                equals e.SourceReference
            join cl in dbContext.DU_CourseLocations on
                Convert.ToString(c.UKPRN) + "_" + c.CourseId + "_" + Convert.ToString((int)c.StudyMode)
                equals Convert.ToString(cl.UKPRN) + "_" + cl.CourseId + "_" + Convert.ToString((int)cl.StudyMode)
                into courseLocations
            from cl in courseLocations.DefaultIfEmpty()
            join l in dbContext.Locations on 
                Convert.ToString(cl.UKPRN) + "_" + cl.LocationId equals l.SourceReference
                into joinedLocations
            from l in joinedLocations.DefaultIfEmpty()
            
            select new EntryInstance()
            {
                Created = DateTime.Now,
                Updated = DateTime.Now,
                Duration = c.NumberOfYears.GetValueOrDefault(0) > 0 ? 
                    TimeSpan.FromDays(c.NumberOfYears.GetValueOrDefault(0) * 365) : null,
                SourceReference = $"{cl.UKPRN}_{cl.CourseId}_{cl.StudyMode}_{cl.LocationId}",
                SourceSystem = SourceSystem.DiscoverUni,
                Reference = $"{cl.UKPRN}_{cl.CourseId}_{cl.StudyMode}_{cl.LocationId}",
                EntryId = e.Id,
                LocationId = l != null ? l.Id : null,
                StudyMode = c.DistanceLearning != null ? c.DistanceLearning.Value.ToStudyMode() : null
            };

        Console.WriteLine("Generating entry instances for other courses...");
        await dbContext.BulkSynchronizeAsync(courseRuns.Distinct(), options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = p => new
            {
                p.Id,
                p.Created,
                p.Updated
            };
            options.UseRowsAffected = true;
            options.ColumnPrimaryKeyExpression = e => e.SourceReference;
            options.ColumnSynchronizeDeleteKeySubsetExpression = e => e.SourceSystem;
            options.ResultInfo = resultInfo;
        }, cancellationToken);
        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();


        // SECTORS
        Console.WriteLine("Generating sectors...");

        var sectors1 =
            from h in dbContext.DU_HECOS 
            join c in dbContext.DU_Courses on
                h.Code equals c.Hecos
            select new Sector()
            {
                SourceSystem = SourceSystem.DiscoverUni,
                Name = h.Label
            };
        var sectors2 =
            from h in dbContext.DU_HECOS 
            join c in dbContext.DU_Courses on
                h.Code equals c.Hecos2
            select new Sector()
            {
                SourceSystem = SourceSystem.DiscoverUni,
                Name = h.Label
            };
        var sectors3 =
            from h in dbContext.DU_HECOS 
            join c in dbContext.DU_Courses on
                h.Code equals c.Hecos3
            select new Sector()
            {
                SourceSystem = SourceSystem.DiscoverUni,
                Name = h.Label
            };
        var sectors4 =
            from h in dbContext.DU_HECOS 
            join c in dbContext.DU_Courses on
                h.Code equals c.Hecos4
            select new Sector()
            {
                SourceSystem = SourceSystem.DiscoverUni,
                Name = h.Label
            };
        var sectors5 =
            from h in dbContext.DU_HECOS 
            join c in dbContext.DU_Courses on
                h.Code equals c.Hecos5
            select new Sector()
            {
                SourceSystem = SourceSystem.DiscoverUni,
                Name = h.Label
            };

        var joined = sectors1.ToDictionary(s => s.Name);
        foreach (var sector in sectors2)
        {
            joined[sector.Name] = sector;
        }
        foreach (var sector in sectors3)
        {
            joined[sector.Name] = sector;
        }
        foreach (var sector in sectors4)
        {
            joined[sector.Name] = sector;
        }
        foreach (var sector in sectors5)
        {
            joined[sector.Name] = sector;
        }

        var distinctSectors = joined.Values;
        await dbContext.BulkSynchronizeAsync(distinctSectors, options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = p => new
            {
                p.Id,
                p.SourceSystem
            };
            options.ColumnPrimaryKeyExpression = p => p.Name;
            options.UseRowsAffected = true;
            options.ResultInfo = resultInfo;
            options.ColumnSynchronizeDeleteKeySubsetExpression = e => e.SourceSystem;
        }, cancellationToken);

        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();

        // ENTRY SECTORS

        Console.WriteLine("Generating entry sectors...");

        var sectors = dbContext.Sectors;
        var hecosSectors = 
            from c in dbContext.DU_Courses
            join e in dbContext.Entries on
                $"{c.UKPRN}_{c.CourseId}_{c.StudyMode}" equals e.SourceReference
            join h in dbContext.DU_HECOS on
                c.Hecos equals h.Code into hecos1
            join h in dbContext.DU_HECOS on
                c.Hecos2 equals h.Code into hecos2
            join h in dbContext.DU_HECOS on
                c.Hecos3 equals h.Code into hecos3
            join h in dbContext.DU_HECOS on
                c.Hecos4 equals h.Code into hecos4
            join h in dbContext.DU_HECOS on
                c.Hecos5 equals h.Code into hecos5
            from h1 in hecos1.DefaultIfEmpty()
            from h2 in hecos2.DefaultIfEmpty()
            from h3 in hecos3.DefaultIfEmpty()
            from h4 in hecos4.DefaultIfEmpty()
            from h5 in hecos5.DefaultIfEmpty()
            select new
            {
                e.Id, Sectors = new List<string?>
                {
                    h1 != null ? h1.Label : null, 
                    h2 != null ? h2.Label : null,
                    h3 != null ? h3.Label : null,
                    h4 != null ? h4.Label : null,
                    h5 != null ? h5.Label : null
                }.Where(x => x != null).Distinct()
            };

        var distinctHecosSectors = hecosSectors.ToList().Distinct();

        var less = "more";
        /*
        

        await dbContext.BulkSynchronizeAsync(entrySectors, options =>
        {
            options.ColumnPrimaryKeyExpression = es => new
            {
                es.EntryId, es.SectorId
            };
            options.UseRowsAffected = true;
            options.ResultInfo = resultInfo;
            options.ColumnSynchronizeDeleteKeySubsetExpression = e => e.SourceSystem;
        }, cancellationToken);

        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();
        
        */

        Console.WriteLine($"{Name} Sync Done");

        return true;
    }

    public override async Task<bool> IndexAsync(CancellationToken cancellationToken)
    {
        return false;
        
        while (true)
        {
            Console.WriteLine($"Starting {Name} AI Search indexing...");

            var entries = dbContext.Entries.Where(x => x.SourceSystem == SourceSystem.DiscoverUni)
                .Include(entry => entry.EntrySectors)
                .ThenInclude(entrySector => entrySector.Sector)
                .Include(entry => entry.EntryInstances)
                .ThenInclude(instance => instance.Location)
                .Include(entry => entry.Provider)
                .ThenInclude(provider => provider.ProviderLocations)
                .ThenInclude(providerLocation => providerLocation.Location)
                .Where(x => x.IngestionState == IngestionState.Pending)
                .Take(50);

            if (!entries.Any())
            {
                Console.WriteLine("No entries found to index.");
                return true;
            }

            Console.WriteLine($"Loaded {entries.Count()} entries for indexing.");

            var searchEntries = new List<AiSearchEntry>();

            foreach (var entry in entries)
            {
                foreach (var instance in entry.EntryInstances)
                {
                    var location = instance.Location ?? entry.Provider.ProviderLocations.FirstOrDefault()?.Location;
                    var searchEntry = new AiSearchEntry
                    {
                        Id = entry.Id.ToString(),
                        InstanceId = instance.LocationId != null
                            ? $"{instance.Id}_{instance.LocationId}"
                            : $"{instance.Id}",
                        Sector = string.Join(", ", entry.EntrySectors.Select(es => es.Sector.Name)),
                        Title = entry.Title,
                        LearningAimTitle = entry.AimOrAltTitle,
                        Description = entry.Description.Scrub(),
                        EntryType = nameof(EntryType.Course),
                        Source = nameof(SourceSystem.DiscoverUni),
                        QualificationLevel = entry.Level?.ToString() ?? string.Empty,
                        LearningMethod = instance.StudyMode.ToString() ?? string.Empty,
                        CourseHours = entry.AttendancePattern?.ToString() ?? string.Empty,
                        StudyTime = entry.StudyTime?.ToString() ?? string.Empty,
                        Location = location?.GeoLocation.ToGeographyPoint()
                    };

                    searchEntry.TitleVector = searchIndexHandler.GetVector(searchEntry.Title);
                    searchEntry.DescriptionVector = searchIndexHandler.GetVector(searchEntry.Description);
                    searchEntry.LearningAimTitleVector = searchIndexHandler.GetVector(searchEntry.LearningAimTitle);
                    searchEntry.SectorVector = searchIndexHandler.GetVector(searchEntry.Sector);
                    searchEntries.Add(searchEntry);
                }

                entry.IngestionState = IngestionState.Processing;
            }

            // Update the entries above to processing
            await dbContext.BulkUpdateAsync(entries, options =>
            {
                options.ColumnInputExpression = e => e.IngestionState;
                options.IncludeGraph = false;
            }, cancellationToken);

            Console.WriteLine($"Created {searchEntries.Count} records for indexing.");

            var result = await searchIndexHandler.Ingest(searchEntries);

            // Update the entries above to complete
            foreach (var entry in entries)
            {
                entry.IngestionState = IngestionState.Complete;
            }

            await dbContext.BulkUpdateAsync(entries, options =>
            {
                options.ColumnInputExpression = e => e.IngestionState;
                options.IncludeGraph = false;
            }, cancellationToken);

            Console.WriteLine($"{Name} AI Search indexing {(result ? "complete" : "failed")}.");


            // Keep going until we've ingested everything
            if (!dbContext.Entries.Any(e =>
                    e.IngestionState == IngestionState.Pending && e.SourceSystem == SourceSystem.DiscoverUni))
                return result;
            dbContext.ChangeTracker.Clear();

        }
    }
}