using feat.web.Models.ViewModels;
using feat.web.Pages;
using feat.web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Session;
using NSubstitute;

namespace feat.web.tests.PageTests;

public class CoursePageTests
{
    private readonly ISearchService _searchService;
    private readonly CourseModel _model;

    public CoursePageTests()
    {
        _searchService = Substitute.For<ISearchService>();
        var session = new TestSession();
        
        var httpContext = new DefaultHttpContext();
        httpContext.Features.Set<ISessionFeature>(new SessionFeature { Session = session });
        
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = httpContext;

        var staticNavigation = new StaticNavigationHandler(httpContextAccessor);

        _model = new CourseModel(_searchService, staticNavigation)
        {
            PageContext = new PageContext
            {
                HttpContext = httpContext
            }
        };
    }

    [Fact]
    public async Task OnGet_Returns_NotFound_When_Course_DoesNotExist()
    {
        var instanceId = Guid.NewGuid();
        _searchService.GetCourseDetails(instanceId.ToString())
            .Returns(Task.FromResult<CourseDetailsBase?>(null));
        
        var result = await _model.OnGet(instanceId);
        
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnGet_Returns_Page_And_Populates_Course_Details()
    {
        var instanceId = Guid.NewGuid();
        var expectedCourse = new CourseDetailsCourse
        {
            Title = "Potato Farmer",
            ProviderName = "Farming Corps"
        };

        _searchService.GetCourseDetails(instanceId.ToString())
            .Returns(Task.FromResult<CourseDetailsBase?>(expectedCourse));
        
        var result = await _model.OnGet(instanceId);
        
        Assert.IsType<PageResult>(result);
        Assert.NotNull(_model.Course);
        Assert.Equal("Potato Farmer", _model.Course.Title);
        Assert.IsType<CourseDetailsCourse>(_model.Course);
    }

    [Fact]
    public async Task OnGet_Sets_RefererUrl_From_NavigationHandler()
    {
        var instanceId = Guid.NewGuid();
        var testReferer = "/previous-page";
        
        _model.HttpContext.Request.Headers.Referer = testReferer;
        
        _searchService.GetCourseDetails(Arg.Any<string>())
            .Returns(Task.FromResult<CourseDetailsBase?>(new CourseDetailsCourse { Title = "Test" }));
        
        await _model.OnGet(instanceId);
        
        Assert.Equal(testReferer, _model.RefererUrl);
    }

    [Fact]
    public async Task OnGet_Handles_Apprenticeship_Type()
    {
        var instanceId = Guid.NewGuid();
        var apprenticeship = new CourseDetailsApprenticeship
        {
            Title = "Plumber Apprenticeship",
            EmployerName = "Plumbers R Us"
        };

        _searchService.GetCourseDetails(instanceId.ToString())
            .Returns(Task.FromResult<CourseDetailsBase?>(apprenticeship));
        
        await _model.OnGet(instanceId);
        
        Assert.IsType<CourseDetailsApprenticeship>(_model.Course);
        var actual = (CourseDetailsApprenticeship)_model.Course!;
        Assert.Equal("Plumbers R Us", actual.EmployerName);
    }
}