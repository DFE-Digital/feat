using feat.api.Models;
using NSubstitute;
using NUnit.Framework;
using System.Net;

namespace feat.api.integrationtests.Endpoints;

[TestFixture]
public class CoursesEndpointTests
{
    private const string Endpoint = "/api/courses";
    
    private ApiWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new ApiWebApplicationFactory();
        _client = _factory.CreateClientWithDefaults();
    }

    [Test]
    public async Task Get_Courses_Returns200WhenFound()
    {
        var instanceId = Guid.NewGuid();

        _factory.CourseService.GetCourseByInstanceIdAsync(instanceId)
            .Returns(new CourseDetailsResponse { Id = instanceId, Title = "Test Course" });

        var response = await _client.GetAsync($"{Endpoint}/{instanceId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Get_Courses_Returns404WhenNotFound()
    {
        var instanceId = Guid.NewGuid();

        _factory.CourseService.GetCourseByInstanceIdAsync(instanceId)
            .Returns((CourseDetailsResponse?)null);

        var response = await _client.GetAsync($"{Endpoint}/{instanceId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}