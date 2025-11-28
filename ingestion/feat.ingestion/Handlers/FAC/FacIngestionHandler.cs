using System.Globalization;
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
using feat.ingestion.Models.FAC;
using feat.ingestion.Models.FAC.Enums;
using Microsoft.EntityFrameworkCore;
using Z.BulkOperations;
using Location = feat.common.Models.Location;
using Provider = feat.common.Models.Provider;

namespace feat.ingestion.Handlers.FAC;

public class FacIngestionHandler(
    IngestionOptions options, 
    IngestionDbContext dbContext,
    ISearchIndexHandler searchIndexHandler,
    BlobServiceClient blobServiceClient) 
    : IngestionHandler(options)
{
    public override IngestionType IngestionType => IngestionType.Csv | IngestionType.Manual;
    public override string Name => "Find A Course";
    public override string Description => "File based ingestion from Find A Course, Publish To Course Directory, and Learning AIM Datasets Manually Uploaded into Blob Storage";
    public override SourceSystem SourceSystem => SourceSystem.FAC;

    private const string ContainerName = "fac";
    
    
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

        var foundAllCourses = false;
        var foundCourses = false;
        var foundTLevels = false;
        var foundAIMData = false;

        await foreach (var blob in files)
        {
            if (blob.Name.Contains("Courses_", StringComparison.InvariantCultureIgnoreCase))
            {
                foundCourses = true;
            }

            if (blob.Name.Contains("TLevels_", StringComparison.InvariantCultureIgnoreCase))
            {
                foundTLevels = true;
            }

            if (blob.Name.Contains("AllCourses", StringComparison.InvariantCultureIgnoreCase))
            {
                foundAllCourses = true;
            }

            if (blob.Name.Contains("LearningDelivery", StringComparison.InvariantCultureIgnoreCase))
            {
                foundAIMData = true;
            }
        }

        if (!foundAllCourses)
        {
            Console.WriteLine("Unable to find all courses data file");
            return false;
        }

        if (!foundCourses)
        {
            Console.WriteLine("Unable to find course text data file");
            return false;
        }

        if (!foundTLevels)
        {
            Console.WriteLine("Unable to find t-level text data file");
            return false;
        }

        if (!foundAIMData)
        {
            Console.WriteLine("Unable to find AIM data file");
            return false;
        }

        // Otherwise, we're returning true
        return true;

    }

    public override async Task<bool> IngestAsync(CancellationToken cancellationToken)
    {
        var Aim = ProcessMode.Skip;
        var ApprovedQualifications = ProcessMode.Skip;
        var Courses = ProcessMode.Skip;
        var TLevels = ProcessMode.Skip;
        var AllCourses = ProcessMode.Skip;
        var Providers = ProcessMode.Skip;
        var Venues = ProcessMode.Skip;
        var Postcodes = ProcessMode.Skip;
        const int batchSize = 5000;

        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var valid = await ValidateAsync(cancellationToken);

        // If we're not passing validation, stop
        if (!valid)
        {
            Console.WriteLine("Unable to validate course data, stopping");
            return false;
        }

        // Let's stream our data in
        var files = containerClient.GetBlobsAsync(cancellationToken: cancellationToken)
            .ToBlockingEnumerable(cancellationToken).ToArray();

        // Get our latest postcode Data file
        var postcodeData = files.Where(blob =>
                blob.Name.StartsWith("ukpostcodes", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (postcodeData != null && Postcodes != ProcessMode.Skip)
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

            if (
                Postcodes == ProcessMode.Force
                || dbContext.Postcodes.Count() != records.Count
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

            Console.WriteLine("Done");
        }
        
        // Get our latest AIM Data file
        var aimData = files.Where(blob =>
                blob.Name.StartsWith("LearningDelivery_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (aimData != null && Aim != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of AIM Data");
            var blobClient = containerClient.GetBlobClient(aimData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<AimDataMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<AimData>().ToList();

            if (
                Aim == ProcessMode.Force
                || dbContext.FAC_AimData.Count() != records.Count
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

            Console.WriteLine("Done");
        }

        // Get our latest Approved Qualification data
        var approvedQualificationData = files.Where(blob =>
                blob.Name.StartsWith("ApprovedQualifications_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (approvedQualificationData != null && ApprovedQualifications != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of Approved Qualification Data");
            var blobClient = containerClient.GetBlobClient(approvedQualificationData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<ApprovedQualificationMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<ApprovedQualification>().ToList();

            if (
                ApprovedQualifications == ProcessMode.Force
                || dbContext.FAC_ApprovedQualifications.Count() != records.Count
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

            Console.WriteLine("Done");
        }
        
        // Get our latest courses file
        var courseData = files.Where(blob =>
                blob.Name.StartsWith("Courses_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (courseData != null && Courses != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of Course Data");
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
                var lastUpdated = records.Max(x => x.UpdatedOn);
                var lastCreated = records.Max(x => x.CreatedOn);

                if (
                    Courses == ProcessMode.Force
                    || dbContext.FAC_Courses.Count() != records.Count
                    || dbContext.FAC_Courses.Max(x => x.UpdatedOn) < lastUpdated
                    || dbContext.FAC_Courses.Max(x => x.CreatedOn) < lastCreated
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
        
        // Get our latest course runs file
        var courseRunData = files.Where(blob =>
                blob.Name.StartsWith("CourseRuns_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (courseRunData != null && Courses != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of Course Run Data");
            var blobClient = containerClient.GetBlobClient(courseRunData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<CourseRunMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<CourseRun>().ToList();

            // Determine if there are any changes
            if (records.Count != 0)
            {
                var lastUpdated = records.Max(x => x.UpdatedOn);
                var lastCreated = records.Max(x => x.CreatedOn);

                if (
                    Courses == ProcessMode.Force
                    || dbContext.FAC_CourseRuns.Count() != records.Count
                    || dbContext.FAC_CourseRuns.Max(x => x.UpdatedOn) < lastUpdated
                    || dbContext.FAC_CourseRuns.Max(x => x.CreatedOn) < lastCreated
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

        // Get our latest T Levels file
        var tLevelData = files.Where(blob =>
                blob.Name.StartsWith("TLevels_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (tLevelData != null && TLevels != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of T-Level Data");

            var blobClient = containerClient.GetBlobClient(tLevelData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<TLevelMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<TLevel>().ToList();
            var lastUpdated = records.Max(x => x.UpdatedOn);
            var lastCreated = records.Max(x => x.CreatedOn);
            var lastDeleted = records.Max(x => x.DeletedOn);

            if (
                TLevels == ProcessMode.Force
                || dbContext.FAC_TLevels.Count() != records.Count
                || dbContext.FAC_TLevels.Max(x => x.UpdatedOn) < lastUpdated
                || dbContext.FAC_TLevels.Max(x => x.CreatedOn) < lastCreated
                || dbContext.FAC_TLevels.Max(x => x.DeletedOn) < lastDeleted
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

            Console.WriteLine("Done");

        }
        
        // Get our latest T Level Definitions file
        var tLevelDefinitionData = files.Where(blob =>
                blob.Name.StartsWith("TLevelDefinitions_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (tLevelDefinitionData != null && TLevels != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of T-Level Definition Data");

            var blobClient = containerClient.GetBlobClient(tLevelDefinitionData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<TLevelDefinitionMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<TLevelDefinition>().ToList();

            if (
                TLevels == ProcessMode.Force
                || dbContext.FAC_TLevels.Count() != records.Count
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

            Console.WriteLine("Done");

        }
        
        // Get our latest T Level Locations file
        var tLevelLocationData = files.Where(blob =>
                blob.Name.StartsWith("TLevelLocations_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (tLevelLocationData != null && TLevels != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of T-Level Location Data");

            var blobClient = containerClient.GetBlobClient(tLevelLocationData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<TLevelLocationMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<TLevelLocation>().ToList();

            if (
                TLevels == ProcessMode.Force
                || dbContext.FAC_TLevelLocations.Count() != records.Count
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

            Console.WriteLine("Done");

        }
        
        // Get our latest Providers file
        var providerData = files.Where(blob =>
                blob.Name.StartsWith("Providers_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (providerData != null && Providers != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of Provider Data");

            var blobClient = containerClient.GetBlobClient(providerData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<ProviderMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<feat.ingestion.Models.FAC.Provider>().ToList();
            var lastUpdated = records.Max(x => x.UpdatedOn);

            if (
                Providers == ProcessMode.Force
                || dbContext.FAC_Providers.Count() != records.Count
                || dbContext.FAC_Providers.Max(x => x.UpdatedOn) < lastUpdated
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

            Console.WriteLine("Done");

        }

        // Get our latest all courses file
        var allCoursesData = files.Where(blob =>
                blob.Name.StartsWith("AllCoursesReport_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (allCoursesData != null && AllCourses != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of All Course Report Data");

            var blobClient = containerClient.GetBlobClient(allCoursesData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            Console.WriteLine("Reading data...");
            csv.Context.RegisterClassMap<AllCoursesCourseMap>();
            var records = csv.GetRecords<AllCoursesCourse>().ToList();
            var lastUpdated = records.Max(x => x.CREATED_DATE);
            var lastCreated = records.Max(x => x.UPDATED_DATE);

            if (
                AllCourses == ProcessMode.Force
                || dbContext.FAC_AllCourses.Count() != records.Count
                || dbContext.FAC_AllCourses.Max(x => x.UPDATED_DATE) < lastUpdated
                || dbContext.FAC_AllCourses.Max(x => x.CREATED_DATE) < lastCreated
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

            Console.WriteLine("Done");

        }
        
        // Get our latest venue file
        var venueData = files.Where(blob =>
                blob.Name.StartsWith("Venues_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (venueData != null && Venues != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of Venue Data");

            var blobClient = containerClient.GetBlobClient(venueData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<VenueMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<Venue>().ToList();
            var lastUpdated = records.Max(x => x.CreatedOn);
            var lastCreated = records.Max(x => x.UpdatedOn);

            if (
                Venues == ProcessMode.Force
                || dbContext.FAC_Venues.Count() != records.Count
                || dbContext.FAC_Venues.Max(x => x.UpdatedOn) < lastUpdated
                || dbContext.FAC_Venues.Max(x => x.CreatedOn) < lastCreated
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
            from v in dbContext.FAC_Venues
            join p in dbContext.Postcodes
                on v.Postcode equals p.Postcode into postcodes
            from p in postcodes.DefaultIfEmpty()
            where v.VenueStatus == VenueStatus.Live
            select new Location()
            {
                Created = v.CreatedOn.GetValueOrDefault(DateTime.Now),
                Updated = v.UpdatedOn.GetValueOrDefault(DateTime.Now),
                Name = v.VenueName,
                Address1 = v.AddressLine1,
                Address2 = v.AddressLine2,
                Town = v.Town,
                County = v.County,
                Postcode = v.Postcode,
                GeoLocation = p != null ? p.ToPoint() : null,
                Email = v.Email,
                Url = v.Website,
                Telephone = v.Telephone,

                SourceReference = v.VenueId.ToString(),
                SourceSystem = SourceSystem

            };

        await dbContext.BulkSynchronizeAsync(locations, options =>
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
        
        // PROVIDERS
        Console.WriteLine("Generating providers...");
        var providers =
            from p in dbContext.FAC_Providers
            select new Provider()
            {
                SourceSystem = SourceSystem,
                SourceReference = p.ProviderId.ToString(),
                Created = DateTime.Now,
                Updated = p.UpdatedOn ?? DateTime.Now,
                
                Name = p.ProviderName ?? "",
                Ukprn = p.Ukprn.ToString(),
                TradingName = p.TradingName,
                OtherNames = p.Alias
            };
        
        await dbContext.BulkSynchronizeAsync(providers, options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = p => new
            {
                p.Id,
                p.Created,
                p.Updated
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
        
        Console.WriteLine("Generating providers locations...");
        var providerLocations =
            from fp in dbContext.FAC_Providers
            join p in dbContext.Providers
                on fp.ProviderId.ToString() equals p.SourceReference
            join v in dbContext.FAC_Venues
                on fp.ProviderId equals v.ProviderId
            join l in dbContext.Locations
                on v.VenueId.ToString() equals l.SourceReference
            select new ProviderLocation()
            {
                ProviderId = p.Id,
                LocationId = l.Id,
                SourceSystem = SourceSystem
            };
        
        await dbContext.BulkSynchronizeAsync(providerLocations.Distinct(), options =>
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
        
        // ENTRY - T-Levels
        
        var tlevels =
            from c in dbContext.FAC_AllCourses
            join t in dbContext.FAC_TLevels on
                c.COURSE_ID equals t.TLevelId
            join td in dbContext.FAC_TLevelDefinitions on
                t.TLevelDefinitionId equals td.TLevelDefinitionId
            join p in dbContext.Providers on
                c.PROVIDER_UKPRN.ToString() equals p.Ukprn
            where c.COURSE_TYPE == CourseType.TLevels && t.TLevelStatus == Status.Live
            select new Entry()
            {
                Created = t.CreatedOn,
                Updated = t.UpdatedOn ?? DateTime.Now,
                SourceReference = c.COURSE_ID.ToString(),
                SourceSystem = SourceSystem,
                SourceUpdated = DateTime.Now,
                Type = EntryType.Course,

                ProviderId = p.Id,

                Title = c.COURSE_NAME ?? td.Name,
                AimOrAltTitle = td.Name,
                Description = t.WhoFor,
                EntryRequirements = t.EntryRequirements,
                WhatYouWillLearn = t.WhatYoullLearn,


                Url = t.Website ?? c.COURSE_URL ?? string.Empty,
                FlexibleStart = c.FLEXIBLE_STARTDATE.GetValueOrDefault(false),
                Reference = c.COURSE_ID.ToString(),
                AttendancePattern = c.STUDY_MODE.ToCourseHours(),
                StudyTime = c.ATTENDANCE_PATTERN.ToStudyTime(),
                Level = td.QualificationLevel.ToQualificationLevel(),
                CourseType = CourseType.TLevels
                
            };

        // ENTRY - Other Courses
        
        var courses = from c in dbContext.FAC_AllCourses
            join a in dbContext.FAC_AimData on 
                c.LEARN_AIM_REF equals a.LearnAimRef into aimdata
            from a in aimdata.DefaultIfEmpty()
            join p in dbContext.Providers on
                c.PROVIDER_UKPRN.ToString() equals p.Ukprn
            join c2 in dbContext.FAC_Courses on
                c.COURSE_ID equals c2.CourseId
            where 
                c.COURSE_TYPE != CourseType.Apprenticeship 
                && c.COURSE_TYPE != CourseType.TLevels
            select new Entry()
            {
                Created = c.CREATED_DATE ?? DateTime.Now,
                Updated = c.UPDATED_DATE ?? DateTime.Now,
                SourceReference = c.COURSE_ID.ToString(),
                SourceSystem = SourceSystem,
                SourceUpdated = DateTime.Now,
                Type = EntryType.Course,

                ProviderId = p.Id,

                Title = c.COURSE_NAME ?? string.Empty,
                AimOrAltTitle = a != null ? a.LearnAimRefTitle : string.Empty,
                Description = c.WHO_THIS_COURSE_IS_FOR,
                EntryRequirements = c.ENTRY_REQUIREMENTS,
                WhatYouWillLearn = c2.WhatYoullLearn,

                Url = c.COURSE_URL ?? string.Empty,
                FlexibleStart = c.FLEXIBLE_STARTDATE.GetValueOrDefault(false),
                Reference = c.COURSE_ID.ToString(),
                AttendancePattern = c.STUDY_MODE.ToCourseHours(),
                StudyTime = c.ATTENDANCE_PATTERN.ToStudyTime(),
                Level = a != null ? a.NotionalNVQLevelv2.ToQualificationLevel() : null,
                CourseType = c.COURSE_TYPE
            };
        
        Console.WriteLine("Generating entries for t-levels...");
        var dictionary = await tlevels.Distinct().ToDictionaryAsync(e => e.SourceReference, cancellationToken: cancellationToken);

        Console.WriteLine($"Generating entries for {courses.LongCount()} other courses...");
        await foreach (var entry in courses.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            dictionary[entry.SourceReference] = entry;
        }
        var distinctEntries = dictionary.Values.ToList();
        
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
        
        // ENTRY INSTANCE - T-Level

        var tLevelInstances =
            from c in dbContext.FAC_AllCourses
            join t in dbContext.FAC_TLevels on
                c.COURSE_ID equals t.TLevelId
            join e in dbContext.Entries on
                c.COURSE_ID.ToString() equals e.SourceReference
            join tl in dbContext.FAC_TLevelLocations on 
                t.TLevelId equals tl.TLevelId
            join l in dbContext.Locations on
                tl.VenueId.ToString() equals l.SourceReference
            where t.TLevelStatus == Status.Live && c.COURSE_TYPE == CourseType.TLevels
            select new EntryInstance()
            {
                Created = t.CreatedOn,
                Updated = t.UpdatedOn,
                Duration = c.DURATION,
                SourceReference = c.COURSE_RUN_ID.ToString(),
                SourceSystem = SourceSystem,
                Reference = c.COURSE_RUN_ID.ToString(),
                StartDate = c.STARTDATE,
                EntryId = e.Id,
                LocationId = l.Id,
                StudyMode = c.DELIVER_MODE.ToLearningMethod()
            };
                
        var courseRuns =
            from c in dbContext.FAC_AllCourses
            join cr in dbContext.FAC_CourseRuns on
                c.COURSE_RUN_ID equals cr.CourseRunId
            join l in dbContext.Locations on
                cr.VenueId.ToString() equals l.SourceReference into instanceLocations
            join e in dbContext.Entries on
                c.COURSE_ID.ToString() equals e.SourceReference
            from l in instanceLocations.DefaultIfEmpty()
            where c.COURSE_TYPE != CourseType.TLevels && c.COURSE_TYPE != CourseType.Apprenticeship
            select new EntryInstance()
            {
                Created = c.CREATED_DATE ?? DateTime.Now,
                Updated = c.UPDATED_DATE ?? DateTime.Now,
                Duration = c.DURATION,
                SourceReference = c.COURSE_RUN_ID.ToString(),
                SourceSystem = SourceSystem,
                Reference = c.COURSE_RUN_ID.ToString(),
                StartDate = c.STARTDATE,
                EntryId = e.Id,
                LocationId = l != null ? l.Id : null,
                StudyMode = c.DELIVER_MODE.ToLearningMethod()
            };
        
        Console.WriteLine("Generating entry instances for t-levels...");
        var instanceDictionary = new Dictionary<string, EntryInstance>();
        await foreach (var entry in tLevelInstances.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            instanceDictionary[entry.SourceReference] = entry;
        }
        
        Console.WriteLine("Generating entry instances for other courses...");
        await foreach (var entry in courseRuns.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            instanceDictionary[entry.SourceReference] = entry;
        }
        var distinctInstances = instanceDictionary.Values.ToList();
        
        // ENTRY INSTANCE - Merged List
        
        await dbContext.BulkSynchronizeAsync(distinctInstances, options =>
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
        
        // ENTRY COSTS
        
        Console.WriteLine("Generating entry costs...");
        
        var costs =
            from cr in dbContext.FAC_CourseRuns
            join e in dbContext.Entries on
                cr.CourseId.ToString() equals e.SourceReference
            where 
                cr.Cost.GetValueOrDefault(0) > 0 || cr.CostDescription != null
                
            select new EntryCost()
            {
                EntryId = e.Id,
                Description = cr.CostDescription,
                SourceSystem = SourceSystem,
                Value = cr.Cost != null && cr.Cost.Value > 0 ? decimal.ToDouble(cr.Cost.Value) : null
            };
        
        await dbContext.BulkSynchronizeAsync(costs, options =>
        {
            options.ColumnPrimaryKeyExpression = p => new
            {
                p.EntryId
            };
            options.AllowDuplicateKeys = true;
            options.UseRowsAffected = true;
            options.ResultInfo = resultInfo;
            options.ColumnSynchronizeDeleteKeySubsetExpression = e => e.SourceSystem;
        }, cancellationToken);
        
        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();
        
        
        // SECTORS
        Console.WriteLine("Generating sectors...");
        
        var sectors =
            from c in dbContext.FAC_AllCourses
            where c.SECTOR != null
            select new Sector()
            {
                SourceSystem = SourceSystem,
                Name = c.SECTOR!
            };
        
        var qualificationSectors =
            from aq in dbContext.FAC_ApprovedQualifications
            select new Sector()
            {
                SourceSystem = SourceSystem,
                Name = aq.SectorSubjectArea
            };

        var distinctCourseSectors = sectors.Distinct();
        var distinctQualificationSectors = qualificationSectors.Distinct();
        var distinctSectors = distinctCourseSectors.Union(distinctQualificationSectors);
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
            
        var courseSectors =
            from c in dbContext.FAC_AllCourses
            join e in dbContext.Entries on
                c.COURSE_ID.ToString() equals e.SourceReference
            join s in dbContext.Sectors on 
                c.SECTOR equals s.Name
            select new EntrySector()
            {
                SourceSystem = SourceSystem,
                EntryId = e.Id,
                SectorId = s.Id
            };
        
        var courseSectorsByAim=
            from c in dbContext.FAC_AllCourses
            join e in dbContext.Entries on
                c.COURSE_ID.ToString() equals e.SourceReference
            join aq in dbContext.FAC_ApprovedQualifications on 
                c.LEARN_AIM_REF equals aq.QualificationNumber
            join s in dbContext.Sectors on 
                aq.SectorSubjectArea equals s.Name
            where c.SECTOR == null
            select new EntrySector()
            {
                SourceSystem = SourceSystem,
                EntryId = e.Id,
                SectorId = s.Id
            };
        
        var d1 = courseSectors.Distinct();
        var d2 = courseSectorsByAim.Distinct();
        var entrySectors = d1.Union(d2);
        
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
                .Where(x => x.SourceSystem == SourceSystem && 
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
                        EntryType = nameof(EntryType.Course),
                        Source = nameof(SourceSystem),
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
            await dbContext.BulkSaveChangesAsync(cancellationToken);


            Console.WriteLine($"Created {searchEntries.Count} records for indexing.");

            var result = await searchIndexHandler.Ingest(searchEntries);

            Console.WriteLine($"Indexed {searchEntries.Count} records.");
            
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
                Console.WriteLine($"{Name} AI Search indexing {(result ? "complete" : "failed")}.");
                return result;
            }
            
            // Clear our change tracking as we're going to get another batch next and
            // we don't care about the old ones
            dbContext.ChangeTracker.Clear();

        }
    }
}