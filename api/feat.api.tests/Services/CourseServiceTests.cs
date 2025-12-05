using feat.api.Data;
using feat.api.Services;
using NSubstitute;
using NUnit.Framework;
using ZiggyCreatures.Caching.Fusion;

namespace feat.api.tests.Services;

[TestFixture]
public class CourseServiceTests(
    CourseDbContext dbContext,
    CourseService courseService,
    IFusionCache cache)
{
    private CourseDbContext _dbContext = dbContext;
    private IFusionCache _cache = cache;
    
    private CourseService _courseService = courseService;

    [SetUp]
    public void Setup()
    {
        _dbContext = Substitute.For<CourseDbContext>();
        _cache = Substitute.For<IFusionCache>();
        
        _courseService = new CourseService(_dbContext, _cache);
    }
}