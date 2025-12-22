using System.Text;
using System.Text.Json;
using feat.web.Enums;
using feat.web.Models;
using feat.web.Pages;
using feat.web.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging.Abstractions;

namespace feat.web.tests.PageTests;

public class QualificationLevelPageTests
{
	private static QualificationLevelModel CreateModel(ISession session)
	{
		var httpContext = new DefaultHttpContext
		{
			Session = session
		};

		var model = new QualificationLevelModel(NullLogger<QualificationLevelModel>.Instance)
		{
			PageContext = new PageContext
			{
				HttpContext = httpContext
			},
			Search = new Search(),
		};
		return model;
	}

	[Fact]
	public void OnGet_Populates_SelectedQualificationOptions_From_Session_And_Returns_Page()
	{
		var search = new Search { QualificationLevels = new List<QualificationLevel>
		{
			QualificationLevel.FourToEight, QualificationLevel.OneAndTwo
		}, Updated = true };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));

		var model = CreateModel(session);

		var result = model.OnGet();

		Assert.IsType<PageResult>(result);
		Assert.Contains(QualificationLevel.FourToEight, model.SelectedQualificationOptions);
		Assert.Contains(QualificationLevel.OneAndTwo, model.SelectedQualificationOptions);
	}

	[Fact]
	public void OnPost_When_No_Qualification_Selection_Returns_Page()
	{
		var search = new Search();
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

		var model = CreateModel(session);
		model.SelectedQualificationOptions = new List<QualificationLevel>();

		var result = model.OnPost();

		Assert.IsType<PageResult>(result);
		Assert.False(model.ModelState.IsValid);
		Assert.True(model.ModelState.ContainsKey("SelectedQualificationOptions"));
	}

	[Fact]
	public void OnPost_Selects_Qualification_Is_None_And_Not_VisitedCheckAnswers_Redirects_To_AgePage()
	{
		var search = new Search { VisitedCheckAnswers = false };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

		var model = CreateModel(session);
		model.SelectedQualificationOptions = new List<QualificationLevel> { QualificationLevel.None };

		var result = model.OnPost();

		var redirect = Assert.IsType<RedirectToPageResult>(result);
		Assert.Equal(PageName.Age, redirect.PageName);

		Assert.True(session.TryGetValue("Search", out var data));
		var saved = JsonSerializer.Deserialize<Search>(Encoding.UTF8.GetString(data))!;
		
		Assert.Contains(QualificationLevel.None, saved.QualificationLevels);
		Assert.True(saved.Updated);
	}

	[Fact]
	public void OnPost_VisitedCheckAnswers_True_Selects_Qualification_None_And_AgeGroupSet_Redirects_To_CheckAnswers()
	{
		var search = new Search { VisitedCheckAnswers = true, AgeGroup = AgeGroup.Eighteen }; 
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

		var model = CreateModel(session);
		model.SelectedQualificationOptions = new List<QualificationLevel> { QualificationLevel.None };

		var result = model.OnPost();

		var redirect = Assert.IsType<RedirectToPageResult>(result);
		Assert.Equal(PageName.CheckAnswers, redirect.PageName);
	}

	[Fact]
	public void OnPost_VisitedCheckAnswers_True_When_Selects_Other_Then_Redirects_To_CheckAnswers_And_Clears_AgeGroup()
	{
		var search = new Search { VisitedCheckAnswers = true, AgeGroup = AgeGroup.Eighteen };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

		var model = CreateModel(session);
		model.SelectedQualificationOptions = new List<QualificationLevel> { QualificationLevel.FourToEight };

		var result = model.OnPost();

		var redirect = Assert.IsType<RedirectToPageResult>(result);
		Assert.Equal(PageName.CheckAnswers, redirect.PageName);

		Assert.True(session.TryGetValue("Search", out var data));
		var saved = JsonSerializer.Deserialize<Search>(Encoding.UTF8.GetString(data))!;
		Assert.Null(saved.AgeGroup);
	}
}