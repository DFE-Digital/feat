using feat.api.Data;
using feat.api.Services;
using NSubstitute;
using NUnit.Framework;

namespace feat.api.tests.Services;

[TestFixture]
public class CourseServiceTests(
    IngestionDbContext dbContext,
    CourseService courseService)
{
    private IngestionDbContext _dbContext = dbContext;
    
    private CourseService _courseService = courseService;

    [SetUp]
    public void Setup()
    {
        _dbContext = Substitute.For<IngestionDbContext>();
        
        _courseService = new CourseService(_dbContext);
    }
}