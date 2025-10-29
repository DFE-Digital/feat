using feat.api.Controllers;
using feat.api.Services;
using NSubstitute;
using NUnit.Framework;

namespace feat.api.tests.Controllers;

[TestFixture]
public class CoursesControllerTests
{
    private ICourseService _courseService;
    
    private CoursesController _coursesController;

    [SetUp]
    public void Setup()
    {
        _courseService = Substitute.For<ICourseService>();
        
        _coursesController = new CoursesController(_courseService);
    }
}