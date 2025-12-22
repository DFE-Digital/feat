using System.Text;
using feat.web.Enums;
using feat.web.Models;
using feat.web.Pages;
using feat.web.Utils;
using feat.web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Microsoft.Identity.Client;

namespace feat.web.tests.PageTests;

public class LocationPageTests
{
	private static LocationModel CreateModel(ISession session, ISearchService? searchService = null) 
    {
	    var searchSrv = searchService ?? Mock.Of<ISearchService>();

        var httpContext = new DefaultHttpContext
        {
            Session = session
        };

        var model = new LocationModel(NullLogger<LocationModel>.Instance, searchSrv)
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
	public async Task OnGetAutoCompleteAsync_Returns_AutoComplete_Results_As_Json()
	{
		var session = new TestSession();
		var locations = new[]
		{
			new AutoCompleteLocation { Name = "Sheffield", Latitude = 53.38, Longitude = -1.47 },
			new AutoCompleteLocation { Name = "Sheffield University", Latitude = 53.38, Longitude = -1.49 }
		};

		var mockService = new Mock<ISearchService>();
		mockService.Setup(s => s.GetAutoCompleteLocations(It.Is<string>(q => q == "Shef"), It.IsAny<CancellationToken>()))
			.ReturnsAsync(locations);

		var model = CreateModel(session, mockService.Object);

		var result = await model.OnGetAutoCompleteAsync("Shef");

		var json = Assert.IsType<JsonResult>(result);
		Assert.Same(locations, json.Value);
	}
	
	[Fact]
	public void OnGet_Loads_IndexPage_Into_History_When_Navigating_From_LoadCoursesPage_Returns_Page()
	{
		var search = new Search { Updated = true, History = new List<string>() { PageName.LoadCourses } };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));

		var model = CreateModel(session);

		var result = model.OnGet();

		var hasIndexAndLocationPages = model.Search.History.Contains(PageName.Index) && 
		                               model.Search.History.Contains(PageName.Location);
		Assert.True(hasIndexAndLocationPages);
		Assert.IsType<PageResult>(result);
	}	

	[Fact]
	public void OnGet_Populates_Properties_From_Session_And_Returns_Page()
	{
		var search = new Search { Updated = true, Location = "Sheffield", Distance = Distance.Ten };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));

		var model = CreateModel(session);

		var result = model.OnGet();

		Assert.IsType<PageResult>(result);
		Assert.Equal("Sheffield", model.Location);
		Assert.Equal(Distance.Ten, model.Distance);
	}

	[Fact]
	public async Task OnPost_When_Location_Is_Set_But_No_Distance_Returns_Page()
	{
		//WIP 
		var session = new TestSession();

		var locations = new[]
		{
			new AutoCompleteLocation { Name = "Sheffield", Latitude = 1.1, Longitude = 2.2 },
			new AutoCompleteLocation { Name = "Sheffield University", Latitude = 1.2, Longitude = 2.3 }
		};
		var mockService = new Mock<ISearchService>();
		mockService.Setup(s => s.GetAutoCompleteLocations(It.Is<string>(q => q == "Shef"), It.IsAny<CancellationToken>()))
			.ReturnsAsync(locations);
		
		var model = CreateModel(session, mockService.Object);
		
		model.Location = "Sheffield"; // distance not set

		var result = await model.OnPost();

		Assert.IsType<PageResult>(result);
		Assert.False(model.ModelState.IsValid);
		Assert.True(model.ModelState.ContainsKey("Distance"));
	}

	[Fact]
	public async Task OnPost_Distance_Is_Set_With_No_Location_Returns_Page()
	{
		var session = new TestSession();
		
		var mockSvc = new Mock<ISearchService>();
		mockSvc.Setup(s => s.IsLocationValid(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
		
		var model = CreateModel(session, mockSvc.Object);
		model.Distance = Distance.Ten;

		var result = await model.OnPost();

		Assert.IsType<PageResult>(result);
		Assert.False(model.ModelState.IsValid);
		Assert.True(model.ModelState.ContainsKey("Location"));
	}
	
	[Fact]
	public async Task OnPost_Invalid_Location_Returns_ModelError_LocationNotFound()
	{
		var search = new Search { VisitedCheckAnswers = false };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));

		var mockSvc = new Mock<ISearchService>();
		mockSvc.Setup(s => s.IsLocationValid(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

		var model = CreateModel(session, mockSvc.Object);
		model.Location = "UnkownTown";

		var result = await model.OnPost();

		Assert.IsType<PageResult>(result);
		Assert.False(model.ModelState.IsValid);
		Assert.True(model.ModelState.ContainsKey("Location"));
		
		var entry = model.ModelState["Location"];
		Assert.Contains(SharedStrings.LocationNotFound, entry.Errors.Select(e => e.ErrorMessage));
	}

	[Fact]
	public async Task OnPost_Not_VisitedCheckAnswers_Redirects_To_InterestsPage_()
	{
		var search = new Search { VisitedCheckAnswers = false };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));
		
		var mockSvc = new Mock<ISearchService>();
		mockSvc.Setup(s => s.IsLocationValid(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
		
		var model = CreateModel(session, mockSvc.Object);
		model.Location = "Sheffield";
		model.Distance = Distance.Ten;
		
		var result = await model.OnPost();

		var redirect = Assert.IsType<RedirectToPageResult>(result);
		Assert.Equal(PageName.Interests, redirect.PageName);
	}

	[Fact]
	public async Task OnPost_VisitedCheckAnswers_And_Distance_IsNot_ThirtyPlus_Redirects_To_CheckAnswers()
	{
		var search = new Search { VisitedCheckAnswers = true };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));

		var mockSvc = new Mock<ISearchService>();
		mockSvc.Setup(s => s.IsLocationValid(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
		
		var model = CreateModel(session, mockSvc.Object);
		model.Location = "Sheffield";
		model.Distance = Distance.Ten;

		var result = await model.OnPost();

		var redirect = Assert.IsType<RedirectToPageResult>(result);
		Assert.Equal(PageName.CheckAnswers, redirect.PageName);
	}

	[Fact]
	public async Task OnPost_VisitedCheckedAnswers_And_Location_And_Distance_ThirtyPlus_And_No_Interests_Redirects_To_InterestsPage()
	{
		var search = new Search { VisitedCheckAnswers = true, Interests = new List<string>() };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));

		var mockSvc = new Mock<ISearchService>();
		mockSvc.Setup(s => s.IsLocationValid(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
		
		var model = CreateModel(session, mockSvc.Object);
		model.Location = "Sheffield";
		model.Distance = Distance.ThirtyPlus;

		var result = await model.OnPost();

		var redirect = Assert.IsType<RedirectToPageResult>(result);
		Assert.Equal(PageName.Interests, redirect.PageName);
	}

	[Fact]
	public async Task OnPost_VisitedCheckedAnswers_And_Location_And_Distance15_And_Has_Interests_Redirects_To_CheckAnswers()
	{
		var search = new Search { VisitedCheckAnswers = true, Interests = new List<string> { "Maths", "Riemann zeta function" } };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));

		var mockSvc = new Mock<ISearchService>();
		mockSvc.Setup(s => s.IsLocationValid(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
		
		var model = CreateModel(session, mockSvc.Object);
		model.Location = "Sheffield";
		model.Distance = Distance.Fifteen; 

		var result = await model.OnPost();

		var redirect = Assert.IsType<RedirectToPageResult>(result);
		Assert.Equal(PageName.CheckAnswers, redirect.PageName); // "Interests exist, redirect to CheckAnswers page"
	}
	
	[Fact]
	public async Task OnPost_VisitedCheckedAnswers_And_Distance_Is_Null_And_Has_Interests_ReturnsPageResult()
	{
		var search = new Search { VisitedCheckAnswers = true, Interests = new List<string> { "Maths", "Riemann zeta function" } };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));

		var mockSvc = new Mock<ISearchService>();
		mockSvc.Setup(s => s.IsLocationValid(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
		
		var model = CreateModel(session, mockSvc.Object);
		model.Location = "Sheffield";
		model.Distance = null; 

		var result = await model.OnPost();
		
		Assert.IsType<PageResult>(result);
		Assert.True(model.ModelState.ContainsKey("Distance"));
		Assert.False(model.ModelState.IsValid, "ModelState is invalid");
	}
	
	[Fact]
	public async Task WbApplicationFactory()
	{
		var factory = new WebApplicationFactory<Program>();
		var client = factory.CreateClient();
		
		var response = await client.GetAsync("/");
		
	}
}