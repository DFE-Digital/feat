using System.Text;
using feat.web.Enums;
using feat.web.Models;
using feat.web.Pages;
using feat.web.Services;
using feat.web.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Testing;
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
		var sheffield = new AutoCompleteLocation() { Name = "Sheffield", Latitude = 0, Longitude = 0 };

		searchService
			.GetAutoCompleteLocations("Sheffield", Arg.Any<CancellationToken>())
			.Returns([sheffield]);

		searchService.IsLocationValid("Sheffield", Arg.Any<CancellationToken>())
			.Returns(true);

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
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));

		var model = CreateModel(session);

		var result = model.OnGet();

		Assert.IsType<PageResult>(result);
		Assert.Equal("Sheffield", model.Location);
		Assert.Equal(Distance.Ten, model.Distance);
	}

	[Fact]
	public async Task OnPost_Returns_Page_When_Location_Is_Set_But_No_Distance()
	{
		var session = new TestSession();
		var model = CreateModel(session);

		model.Location = "Sheffield"; // distance not set

		var result = await model.OnPost();
		
		Assert.IsType<PageResult>(result);
		Assert.False(model.ModelState.IsValid);
		Assert.True(model.ModelState.ContainsKey("Distance"));
	}

	[Fact]
	public async Task OnPost_Returns_Page_When_Distance_IsSet_But_No_Location()
	{
		var session = new TestSession();
		var model = CreateModel(session);

		model.Distance = Distance.Ten;

		var result = await model.OnPost();
		
		Assert.IsType<PageResult>(result);
		Assert.False(model.ModelState.IsValid);
		Assert.True(model.ModelState.ContainsKey("Location"));
	}

	[Fact]
	public async Task OnPost_Redirects_To_InterestsPage_When_Not_VisitedCheckAnswers()
	{
		var search = new Search { VisitedCheckAnswers = false };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));

		var model = CreateModel(session);
		model.Location = "Sheffield";
		model.Distance = Distance.Ten;

		var result = await model.OnPost();
		
		var redirect = Assert.IsType<RedirectToPageResult>(result);
		Assert.Equal(PageName.Interests, redirect.PageName);
	}

	[Fact]
	public async Task OnPost_Redirects_To_CheckAnswers_When_VisitedCheckAnswers_And_Distance_IsNot_ThirtyPlus()
	{
		var search = new Search { VisitedCheckAnswers = true };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));

		var model = CreateModel(session);
		model.Location = "Sheffield";
		model.Distance = Distance.Ten;

		var result = await model.OnPost();

		var redirect = Assert.IsType<RedirectToPageResult>(result);
		Assert.Equal(PageName.CheckAnswers, redirect.PageName);
	}

	[Fact]
	public async Task OnPost_Redirects_To_InterestsPage_When_VisitedCheckedAnswers_And_Distance_ThirtyPlus_And_No_Interests()
	{
		var search = new Search { VisitedCheckAnswers = true, Interests = new List<string>() };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));

		var model = CreateModel(session);
		model.Location = "Sheffield";
		model.Distance = Distance.ThirtyPlus;

		var result = await model.OnPost();

		var redirect = Assert.IsType<RedirectToPageResult>(result);
		Assert.Equal(PageName.Interests, redirect.PageName);
	}

	[Fact]
	public async Task OnPost_Redirects_To_CheckAnswers_When_VisitedCheckedAnswers_And_Distance15_And_Has_Interests()
	{
		var search = new Search { VisitedCheckAnswers = true, Interests = new List<string> { "Maths", "Riemann zeta function" } };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));

		var model = CreateModel(session);
		model.Location = "Sheffield";
		model.Distance = Distance.Fifteen; 

		var result = await model.OnPost();

		var redirect = Assert.IsType<RedirectToPageResult>(result);
		Assert.Equal(PageName.CheckAnswers, redirect.PageName); // "Interests exist, redirect to CheckAnswers page"
	}
	
	[Fact]
	public async Task OnPost_Redirects_To_CheckAnswers_When_VisitedCheckedAnswers_And_Distance_Is_Null_And_Has_Interests()
	{
		var search = new Search { VisitedCheckAnswers = true, Interests = new List<string> { "Maths", "Riemann zeta function" } };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));

		var model = CreateModel(session);
		model.Location = "Sheffield";
		model.Distance = null;

		var result = await model.OnPost();
		
		Assert.IsType<PageResult>(result);
		Assert.False(model.ModelState.IsValid, "ModelState is invalid");
		Assert.True(model.ModelState.ContainsKey("Distance"));
	}
}