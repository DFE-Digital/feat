using feat.api.Data;
using feat.api.Services;
using NSubstitute;
using NUnit.Framework;

namespace feat.api.tests.Services;

[TestFixture]
public class CourseServiceTests(
    CourseDbContext dbContext,
    CourseService courseService)
{
    private CourseDbContext _dbContext = dbContext;
    
    private CourseService _courseService = courseService;

    [SetUp]
    public void Setup()
    {
        _dbContext = Substitute.For<CourseDbContext>();
        
        _courseService = new CourseService(_dbContext);
    }
}