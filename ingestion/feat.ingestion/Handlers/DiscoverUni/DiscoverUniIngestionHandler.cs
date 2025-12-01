using System.Globalization;
using System.IO.Compression;
using System.Text;
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
    public override IngestionType IngestionType => IngestionType.Compressed | IngestionType.Csv | IngestionType.Manual;
    public override string Name => "Discover Uni";
    public override string Description => "File based ingestion from Discover Uni dataset downloaded from the website";
    public override SourceSystem SourceSystem => SourceSystem.DiscoverUni;

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
        var Aims = ProcessMode.Process;
        var Locations = ProcessMode.Process;
        var Providers = ProcessMode.Process;
        var HECOS = ProcessMode.Process;
        var Courses = ProcessMode.Process;
        
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var valid = await ValidateAsync(cancellationToken);

        const string discoverUniDownload =
            "https://unistatsdatasetdownloadfunction.azurewebsites.net/api/UnistatsDatasetDownload";

        // Let's determine when we last fetched data
        DU.IngestionState ingestionState;

        if (!dbContext.DU_IngestionState.Any())
        {
            ingestionState = new DU.IngestionState
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




        var tempPath = Path.GetTempFileName();
        
        Console.WriteLine("Checking Discover Uni dataset for changes...");
        
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

                Console.WriteLine("Done");
                
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
            csv.Context.RegisterClassMap<AimMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<Aim>().ToList();

            // Determine if there are any changes
            if (records.Count != 0)
            {

                
                    Console.WriteLine($"Preparing {records.Count} records to DB...");
                    await dbContext.BulkSynchronizeAsync(records, options =>
                    {
                        options.BatchSize = batchSize;
                        options.BatchDelayInterval = 1000;
                        options.UseTableLock = true;
                    }, cancellationToken);
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
            csv.Context.RegisterClassMap<HecosMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<Hecos>().ToList();

            // Determine if there are any changes
            if (records.Count != 0)
            {
                Console.WriteLine($"Preparing {records.Count} records to DB...");
                await dbContext.BulkSynchronizeAsync(records, options =>
                {
                    options.BatchSize = batchSize;
                    options.BatchDelayInterval = 1000;
                    options.UseTableLock = true;
                }, cancellationToken);
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
            csv.Context.RegisterClassMap<LocationMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<DU.Location>().ToList();

            // Determine if there are any changes
            if (records.Count != 0)
            {
                Console.WriteLine($"Preparing {records.Count} records to DB...");
                await dbContext.BulkSynchronizeAsync(records, options =>
                {
                    options.BatchSize = batchSize;
                    options.BatchDelayInterval = 1000;
                    options.UseTableLock = true;
                }, cancellationToken);
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
                Console.WriteLine($"Preparing {records.Count} records to DB...");
                await dbContext.BulkSynchronizeAsync(records, options =>
                {
                    options.BatchSize = batchSize;
                    options.BatchDelayInterval = 1000;
                    options.UseTableLock = true;
                }, cancellationToken);
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
            csv.Context.RegisterClassMap<CourseMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<Course>().ToList();

            // Determine if there are any changes
            if (records.Count != 0)
            {
                Console.WriteLine($"Preparing {records.Count} records to DB...");
                await dbContext.BulkSynchronizeAsync(records, options =>
                {
                    options.BatchSize = batchSize;
                    options.BatchDelayInterval = 1000;
                    options.UseTableLock = true;
                }, cancellationToken);
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
            csv.Context.RegisterClassMap<CourseLocationMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<CourseLocation>().ToList();

            // Determine if there are any changes
            if (records.Count != 0)
            {
                Console.WriteLine($"Preparing {records.Count} records to DB...");
                await dbContext.BulkSynchronizeAsync(records, options =>
                {
                    options.BatchSize = batchSize;
                    options.BatchDelayInterval = 1000;
                    options.UseTableLock = true;
                }, cancellationToken);

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
        
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

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
                SourceSystem = SourceSystem
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
            where i.UKPRN == i.PubUKPRN
            select new Provider()
            {
                SourceSystem = SourceSystem,
                SourceReference = i.UKPRN.ToString(),
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
                p.Id
            };
            options.ColumnPrimaryKeyExpression = l => l.Ukprn;
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
                on Convert.ToString(i.UKPRN) equals p.SourceReference
            join ul in dbContext.DU_Locations
                on i.UKPRN equals ul.UKPRN
            join l in dbContext.Locations
                on Convert.ToString(ul.UKPRN) + "_" + ul.LocationId equals l.SourceReference
            where l.SourceSystem == SourceSystem
            && i.UKPRN == i.PubUKPRN
            select new ProviderLocation()
            {
                ProviderId = p.Id,
                LocationId = l.Id,
                SourceSystem = SourceSystem
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
                Convert.ToString(c.UKPRN) equals p.Ukprn
            join a in dbContext.DU_Aims on
                c.Aim equals a.AimCode into aims
            from a in aims.DefaultIfEmpty()
            
            
            select new Entry()
            {
                Created = DateTime.Now,
                Updated = DateTime.Now,
                SourceReference = $"{c.UKPRN}_{c.CourseId}_{(int)c.StudyMode}",
                SourceSystem = SourceSystem,
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
                Url = !string.IsNullOrEmpty(c.CourseUrl) ? c.CourseUrl : string.Empty,
                FlexibleStart = false,
                Level = a != null ?  a.AimCode.ToQualificationLevel() : null,
                Reference = c.CourseId,
                AttendancePattern = c.StudyMode.ToCourseHours(),
                CourseType = CourseType.Degree
            };

        Console.WriteLine($"Generating entries for {courses.LongCount()} courses...");
        var distinctEntries = courses.Distinct();

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
            options.UseAudit = true;
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
        await createdEntries.ForEachAsync(e => e.IngestionState = IngestionState.Pending,
            cancellationToken: cancellationToken);
        // We're only interested here if any text fields have changed
        var updatedIds = auditEntries.Where(e => e.Action == AuditActionType.Update
                                                 && e.Values.Exists(ae =>
                                                     ae.ColumnName is "Title" or "AimOrAltTitle" or "Description" &&
                                                     !Equals(ae.OldValue, ae.NewValue)))
            .SelectMany(e => e.Values.Where(ae => ae.ColumnName == "Id").Select(ae => (Guid)ae.NewValue));
        var updatedEntries = dbContext.Entries.WhereBulkContains(updatedIds);
        await updatedEntries.ForEachAsync(e => e.IngestionState = IngestionState.Pending,
            cancellationToken: cancellationToken);
        await dbContext.BulkSaveChangesAsync(cancellationToken);

        var instances =
            from c in dbContext.DU_Courses
            join e in dbContext.Entries on
                Convert.ToString(c.UKPRN) + "_" + c.CourseId + "_" + Convert.ToString((int)c.StudyMode)
                equals e.SourceReference
            join cl in dbContext.DU_CourseLocations on
                new { c.UKPRN, c.CourseId, c.StudyMode } equals 
                new { cl.UKPRN, cl.CourseId, cl.StudyMode }
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
                SourceReference = cl != null ?
                    $"{c.UKPRN}_{c.CourseId}_{(int)c.StudyMode}_{cl.LocationId}" :
                    $"{c.UKPRN}_{c.CourseId}_{(int)c.StudyMode}",
                SourceSystem = SourceSystem,
                Reference = cl != null ?
                    $"{c.UKPRN}_{c.CourseId}_{(int)c.StudyMode}_{cl.LocationId}" :
                    $"{c.UKPRN}_{c.CourseId}_{(int)c.StudyMode}",
                EntryId = e.Id,
                LocationId = l != null ? l.Id : null,
                StudyMode = c.DistanceLearning != null ? c.DistanceLearning.Value.ToStudyMode() : null
            };
        
        
        var distinctInstances = instances.Distinct();
        
        Console.WriteLine($"Generating entry instances ...");
        await dbContext.BulkSynchronizeAsync(instances, options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = p => new
            {
                p.Id,
                p.Created,
                p.Updated
            };
            options.ColumnPrimaryKeyExpression = e => e.SourceReference;
            options.ColumnSynchronizeDeleteKeySubsetExpression = e => e.SourceSystem;
            options.UseRowsAffected = true;
            options.ResultInfo = resultInfo;
        }, cancellationToken);
        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();

        
        // SECTORS
        Console.WriteLine("Generating sectors...");

        var distinctSectors =
            from h in dbContext.DU_HECOS 
            select new Sector()
            {
                SourceSystem = SourceSystem,
                Name = h.Label
            };
        

        await dbContext.BulkSynchronizeAsync(distinctSectors.Distinct(), options =>
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

        var sectors = (
            from s in dbContext.Sectors
            join h in dbContext.DU_HECOS on
                s.Name equals h.Label
            where s.SourceSystem == SourceSystem
            select new
            {
                Hecos = h, Sector = s
            }).ToList();

        var sectorCourses = (
            from c in dbContext.DU_Courses
            join e in dbContext.Entries on
                Convert.ToString(c.UKPRN) + "_" +
                c.CourseId + "_" +
                Convert.ToString((int)c.StudyMode) equals e.SourceReference
            select new
            {
                EntryId = e.Id, HecosCodes = new List<int?>
                {
                    c.Hecos, c.Hecos2, c.Hecos3, c.Hecos4, c.Hecos5
                }
            }).ToList();

        // Remove empty sectors
        sectorCourses.RemoveAll(s => s.HecosCodes.All(h => h == null));
        var entrySectors = new List<EntrySector>();
        
        sectorCourses.ForEach(c =>
        {
            foreach (var sector in 
                     c.HecosCodes.Select(hecosCode => sectors
                         .FirstOrDefault(s => hecosCode != null &&
                                              s.Hecos.Code == hecosCode.Value)))
            {
                if (sector != null)
                {
                    entrySectors.Add(new EntrySector()
                    {
                        EntryId = c.EntryId,
                        SectorId = sector.Sector.Id,
                        SourceSystem = SourceSystem
                    });
                }
            }
        });
        
        await dbContext.BulkSynchronizeAsync(entrySectors, options =>
        {
            options.ColumnPrimaryKeyExpression = es => new
            {
                es.EntryId, es.SectorId
            };
            options.ColumnSynchronizeDeleteKeySubsetExpression = e => e.SourceSystem;
            options.UseRowsAffected = true;
            options.ResultInfo = resultInfo;
        }, cancellationToken);

        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();
        
        // UNIVERSITY SPECIFIC DATA
        Console.WriteLine("Generating university course specific data...");
        
        var uniData =
            from c in dbContext.DU_Courses
            join e in dbContext.Entries on
                Convert.ToString(c.UKPRN) + "_" +
                c.CourseId + "_" +
                Convert.ToString((int)c.StudyMode) equals e.SourceReference
            select new UniversityCourse()
            {
                EntryId = e.Id,
                Foundation = c.FoundationYear == null ? null : c.FoundationYear.Value != Availability.NotAvailable,
                Honours = c.Honours,
                Sandwich = c.Sandwich == null ? null : c.Sandwich.Value != Availability.NotAvailable,
                Nhs = c.NHS,
                YearAbroad = c.YearAbroad == null ? null : c.YearAbroad.Value != Availability.NotAvailable
            };
        
        await dbContext.BulkSynchronizeAsync(uniData, options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = p => new
            {
                p.Id
            };
            options.ColumnPrimaryKeyExpression = course => course.EntryId;
            options.UseRowsAffected = true;
            options.ResultInfo = resultInfo;
        }, cancellationToken);

        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");


        await transaction.CommitAsync(cancellationToken);
        Console.WriteLine($"{Name} Sync Done");

        return true;
    }

    public override async Task<bool> IndexAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Starting {Name} AI Search indexing...");
        var sb = new StringBuilder();
        
        while (true)
        {
            var entries = dbContext.Entries
                .Include(entry => entry.EntrySectors)
                .ThenInclude(entrySector => entrySector.Sector)
                .Include(entry => entry.EntryInstances)
                .ThenInclude(instance => instance.Location)
                .Include(entry => entry.Provider)
                .ThenInclude(provider => provider.ProviderLocations)
                .ThenInclude(providerLocation => providerLocation.Location)
                .Where(x => 
                    x.SourceSystem == SourceSystem &&
                    x.IngestionState == IngestionState.Pending)
                .Take(250)
                .ToList();

            if (entries.Count == 0)
            {
                Console.WriteLine("No entries found to index.");
                return true;
            }

            Console.WriteLine($"Loaded {entries.Count} entries for indexing.");

            var searchEntries = new List<AiSearchEntry>();
            foreach (var entry in entries)
            {
                // TODO: Split these into their own fields
                sb.Clear();
                sb.AppendLine(entry.Description);
                sb.AppendLine(entry.WhatYouWillLearn);
                var description = sb.ToString().Scrub();
                
                foreach (var instance in entry.EntryInstances)
                {
                    var location = instance.Location ?? entry.Provider.ProviderLocations.FirstOrDefault()?.Location;
                    var searchEntry = new AiSearchEntry
                    {
                        Id = entry.Id.ToString(),
                        InstanceId = instance.Id.ToString(),
                        Sector = string.Join(", ", entry.EntrySectors.Select(es => es.Sector.Name)),
                        Title = entry.Title,
                        LearningAimTitle = entry.AimOrAltTitle,
                        Description = description,
                        EntryType = nameof(EntryType.UniversityCourse),
                        Source = SourceSystem.ToString(),
                        QualificationLevel = entry.Level?.ToString() ?? string.Empty,
                        LearningMethod = instance.StudyMode.ToString() ?? string.Empty,
                        CourseHours = entry.AttendancePattern?.ToString() ?? string.Empty,
                        StudyTime = entry.StudyTime?.ToString() ?? string.Empty,
                        Location = location?.GeoLocation.ToGeographyPoint()
                    };

                    if (options.IndexDirectly)
                    {
                        searchEntry.TitleVector = searchIndexHandler.GetVector(searchEntry.Title);
                        searchEntry.DescriptionVector = searchIndexHandler.GetVector(searchEntry.Description);
                        searchEntry.LearningAimTitleVector = searchIndexHandler.GetVector(searchEntry.LearningAimTitle);
                        searchEntry.SectorVector = searchIndexHandler.GetVector(searchEntry.Sector);
                    }

                    searchEntries.Add(searchEntry);
                }
                
                entry.IngestionState = IngestionState.Processing;
            }

            var resultInfo = new ResultInfo();
            await dbContext.BulkMergeAsync(searchEntries, options =>
            {
                options.ColumnPrimaryKeyExpression = ai => ai.InstanceId;
                options.UseRowsAffected = true;
                options.ResultInfo = resultInfo;
            }, cancellationToken);

            Console.WriteLine($"{resultInfo.RowsAffectedInserted} created for indexing");
            Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated for indexing");
            
            var result = !options.IndexDirectly || await searchIndexHandler.Ingest(searchEntries);
            
            // Update the entries above to processing
            foreach (var entry in entries)
            {
                entry.IngestionState = result ? IngestionState.Complete : IngestionState.Failed;
            }
            await dbContext.BulkSaveChangesAsync(cancellationToken);

            // Keep going until we've ingested everything
            if (!dbContext.Entries.Any(e =>
                    e.IngestionState == IngestionState.Pending
                    && e.SourceSystem == SourceSystem))
            {
                // Clear any AI search entries that aren't in our list of instances
                await dbContext.AiSearchEntries
                    .Where(i => i.Source == SourceSystem.ToString())
                    .WhereBulkNotContains(dbContext.EntryInstances
                        .Select(i => i.Id))
                    .DeleteFromQueryAsync(cancellationToken: cancellationToken);
                
                Console.WriteLine($"{Name} AI Search indexing {(result ? "complete" : "failed")}.");
                return result;
            }

            // Clear our change tracking as we're going to get another batch next and
            // we don't care about the old ones
            dbContext.ChangeTracker.Clear();
        }
    }
}