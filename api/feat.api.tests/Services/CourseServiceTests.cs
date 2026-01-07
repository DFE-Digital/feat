using feat.api.Data;
using feat.api.Models;
using feat.api.Services;
using feat.common.Models;
using feat.common.Models.Enums;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using ZiggyCreatures.Caching.Fusion;

namespace feat.api.tests.Services;

[TestFixture]
public class CourseServiceTests
{
    private CourseDbContext _dbContext;
    private IFusionCache _cache;
    private CourseService _service;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<CourseDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _dbContext = new CourseDbContext(options);
        _cache = Substitute.For<IFusionCache>();
        _service = new CourseService(_dbContext, _cache);

        _cache.GetOrSetAsync(
            Arg.Any<string>(),
            Arg.Any<Func<FusionCacheFactoryExecutionContext<CourseDetailsResponse?>, CancellationToken, Task<CourseDetailsResponse?>>>(),
            Arg.Any<FusionCacheEntryOptions>(),
            Arg.Any<CancellationToken>()
        ).Returns(callInfo =>
        {
            var factory = callInfo.ArgAt<Func<FusionCacheFactoryExecutionContext<CourseDetailsResponse?>, CancellationToken, Task<CourseDetailsResponse?>>>(1);
            return new ValueTask<CourseDetailsResponse?>(factory(null!, CancellationToken.None));
        });
    }
    
    [Test]
    public async Task GetCourseByInstanceIdAsync_ReturnsCachedValue()
    {
        var instanceId = Guid.NewGuid();
        var cachedResponse = new CourseDetailsResponse { Id = instanceId };

        _cache.GetOrSetAsync<CourseDetailsResponse?>(
            $"instance:{instanceId}",
            Arg.Any<Func<FusionCacheFactoryExecutionContext<CourseDetailsResponse?>, CancellationToken, Task<CourseDetailsResponse?>>>()
        ).Returns(new ValueTask<CourseDetailsResponse?>(cachedResponse));
        
        var result = await _service.GetCourseByInstanceIdAsync(instanceId);
        
        Assert.That(result, Is.EqualTo(cachedResponse));
    }
    
    [Test]
    public async Task GetCourseByInstanceIdAsync_MapsApprenticeshipSpecificFields()
    {
        var instanceId = Guid.NewGuid();

        var entry = new Entry
        {
            Id = Guid.NewGuid(),
            Type = EntryType.Apprenticeship,
            Title = "Apprentice Plumber",
            Created = DateTime.Now,
            ProviderId = Guid.NewGuid(),
            Provider = new Provider
            {
                Name = "Plumber Academy",
                Created = DateTime.Now,
                SourceReference = "123"
            },
            Reference = "ABC",
            SourceReference = "123",
            AimOrAltTitle = "Plumber",
            FlexibleStart = false,
            Url = "https://www.google.com/search?q=plumbing"
        };
        
        entry.Vacancies.Add(new Vacancy { 
            Wage = "£15,000 per year", 
            Employer = new Employer
            {
                Name = "Plumbing Ltd",
                Created = DateTime.Now
            }
        });
        
        _dbContext.EntryInstances.Add(new EntryInstance
        {
            Id = instanceId,
            Entry = entry,
            Reference = "ABC",
            SourceReference = "123"
        });
        
        await _dbContext.SaveChangesAsync();

        var result = await _service.GetCourseByInstanceIdAsync(instanceId);
        
        Assert.Multiple(() =>
        {
            Assert.That(result!.Title, Is.EqualTo("Apprentice Plumber"));
            Assert.That(result.Wage, Is.EqualTo("£15,000 per year"));
            Assert.That(result.EmployerName, Is.EqualTo("Plumbing Ltd"));
            Assert.That(result.TrainingProvider, Is.EqualTo("Plumber Academy"));
        });
    }
    
    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }
}