using System.Text;
using System.Text.Json;
using feat.web.Enums;
using feat.web.Models;
using feat.web.Pages;
using feat.web.Services;
using feat.web.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace feat.web.tests.PageTests;

public class LocationPageTests
{
    private static LocationModel CreateModel(ISession session)
    {
        var httpContext = new DefaultHttpContext
        {
            Session = session
        };

        var searchService = Substitute.For<ISearchService>();
            
        searchService
            .GetAutoCompleteLocations("Sheffield", Arg.Any<CancellationToken>())
            .Returns([new AutoCompleteLocation { Name = "Sheffield", Latitude = 0, Longitude = 0 }]);

        searchService.IsLocationValid("Sheffield", Arg.Any<CancellationToken>()).Returns(true);
        searchService.IsLocationValid("Nowhere", Arg.Any<CancellationToken>()).Returns(false);

        var model = new LocationModel(NullLogger<LocationModel>.Instance, searchService)
        {
            PageContext = new PageContext
            {
                HttpContext = httpContext
            },
            Search = new Search()
        };

        return model;
    }
        
    [Fact]
    public void OnGet_Populates_Properties_From_Session_And_Returns_Page()
    {
        var search = new Search { Updated = true, Location = "Sheffield", Distance = Distance.Ten };
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);

        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        Assert.Equal("Sheffield", model.Location);
        Assert.Equal(Distance.Ten, model.Distance);
    }

    [Fact]
    public void OnGet_Resets_Search_When_History_Contains_LoadCourses()
    {
        var search = new Search();
        search.History.Add(PageName.LoadCourses);
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);

        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
            
        Assert.True(session.TryGetValue("Search", out var data));
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var saved = JsonSerializer.Deserialize<Search>(Encoding.UTF8.GetString(data))!;
        Assert.Contains(PageName.Index, saved.History);
        Assert.Null(saved.Location);
        Assert.Null(saved.Distance);
    }
        
    [Fact]
    public async Task OnPost_Returns_Page_When_Location_Invalid()
    {
        var search = new Search { Updated = true };
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);
        model.Location = "Nowhere"; // Invalid
        model.Distance = Distance.Ten;

        var result = await model.OnPost();

        Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
        Assert.True(model.ModelState.ContainsKey("Location"));
    }

    [Fact]
    public async Task OnPost_Returns_Page_When_Location_LessThan3Chars()
    {
        var search = new Search { Updated = true };
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);
        model.Location = "AB"; // <3 chars
        model.Distance = Distance.Ten;

        var result = await model.OnPost();

        Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
        Assert.True(model.ModelState.ContainsKey("Location"));
    }

    [Fact]
    public async Task OnPost_Returns_Page_When_Distance_Zero_And_Location_Set()
    {
        var search = new Search { Updated = true };
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);
        model.Location = "Sheffield";
        model.Distance = 0;

        var result = await model.OnPost();

        Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
        Assert.True(model.ModelState.ContainsKey("Distance"));
    }

    [Fact]
    public async Task OnPost_Returns_Page_When_Distance_Set_But_Location_Empty()
    {
        var search = new Search { Updated = true };
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);
        model.Distance = Distance.Ten;

        var result = await model.OnPost();

        Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
        Assert.True(model.ModelState.ContainsKey("Location"));
    }
        
    [Fact]
    public async Task OnPost_Redirects_To_Interests_When_Not_VisitedCheckAnswers()
    {
        var search = new Search { VisitedCheckAnswers = false };
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);
        model.Location = "Sheffield";
        model.Distance = Distance.Ten;

        var result = await model.OnPost();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(PageName.Interests, redirect.PageName);
    }

    [Fact]
    public async Task OnPost_Redirects_To_CheckAnswers_When_VisitedCheckAnswers_And_Distance_Not_ThirtyPlus()
    {
        var search = new Search { VisitedCheckAnswers = true };
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);
        model.Location = "Sheffield";
        model.Distance = Distance.Ten;

        var result = await model.OnPost();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(PageName.CheckAnswers, redirect.PageName);
    }

    [Fact]
    public async Task OnPost_Redirects_To_Interests_When_VisitedCheckAnswers_And_Distance_ThirtyPlus_And_No_Interests()
    {
        var search = new Search { VisitedCheckAnswers = true, Interests = new List<string>() };
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);
        model.Location = "Sheffield";
        model.Distance = Distance.ThirtyPlus;

        var result = await model.OnPost();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(PageName.Interests, redirect.PageName);
    }

    [Fact]
    public async Task OnPost_Redirects_To_CheckAnswers_When_VisitedCheckAnswers_And_Distance_Not_ThirtyPlus_And_Has_Interests()
    {
        var search = new Search { VisitedCheckAnswers = true, Interests = new List<string> { "Maths", "Riemann zeta function" } };
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);
        model.Location = "Sheffield";
        model.Distance = Distance.Fifteen;

        var result = await model.OnPost();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(PageName.CheckAnswers, redirect.PageName);
    }

    [Fact]
    public async Task OnPost_Returns_Page_When_VisitedCheckAnswers_And_Distance_Null_And_Has_Interests()
    {
        var search = new Search { VisitedCheckAnswers = true, Interests = new List<string> { "Maths", "Riemann zeta function" } };
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);
        model.Location = "Sheffield";
        model.Distance = null;

        var result = await model.OnPost();

        Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
        Assert.True(model.ModelState.ContainsKey("Distance"));
    }
}