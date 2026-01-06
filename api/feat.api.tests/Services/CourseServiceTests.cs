using feat.api.Data;
using feat.api.Models;
using feat.api.Services;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using ZiggyCreatures.Caching.Fusion;

namespace feat.api.tests.Services;

[TestFixture]
public class CourseServiceTests
{
    private CourseService _service;
    private IFusionCache _cache;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<CourseDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        var dbContext = new CourseDbContext(options);
        
        _cache = Substitute.For<IFusionCache>();
        
        _service = new CourseService(
            dbContext,
            _cache);
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
}