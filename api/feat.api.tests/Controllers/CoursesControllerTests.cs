using feat.api.Controllers;
using feat.api.Models;
using feat.api.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace feat.api.tests.Controllers;

[TestFixture]
public class CoursesControllerTests
{
    private ICourseService _courseService;
    private CoursesController _controller;

    [SetUp]
    public void Setup()
    {
        _courseService = Substitute.For<ICourseService>();
        _controller = new CoursesController(_courseService);
    }

    [Test]
    public async Task GetCourseByInstanceId_ReturnsOk_WhenCourseExists()
    {
        var instanceId = Guid.NewGuid();
        var response = new CourseDetailsResponse
        {
            Id = instanceId,
            Title = "Test Course"
        };
        
        _courseService
            .GetCourseByInstanceIdAsync(instanceId)
            .Returns(response);
        
        var result = await _controller.GetCourseByInstanceId(instanceId);
        
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
        Assert.That(okResult!.Value, Is.EqualTo(response));
    }

    [Test]
    public async Task GetCourseByInstanceId_ReturnsNotFound_WhenCourseDoesNotExist()
    {
        var instanceId = Guid.NewGuid();
        
        _courseService
            .GetCourseByInstanceIdAsync(instanceId)
            .Returns((CourseDetailsResponse?)null);
        
        var result = await _controller.GetCourseByInstanceId(instanceId);
        
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result!, Is.TypeOf<NotFoundResult>());
    }
}