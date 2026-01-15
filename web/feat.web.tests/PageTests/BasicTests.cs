using Microsoft.AspNetCore.Mvc.Testing;

namespace feat.web.tests.PageTests;

public class BasicTests: IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BasicTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Index")]
    [InlineData("/Location")]
    [InlineData("/Interests")]
    [InlineData("/QualificationLevel")]
    [InlineData("/Age")]
    [InlineData("/LoadCourses")]
    public async Task Get_Endpoints_ReturnSuccessAndCorrectContentType(string url)
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(url);
        
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }
}