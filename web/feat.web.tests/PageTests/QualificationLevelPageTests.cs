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
		var search = new Search { Updated = true, QualificationLevels = new List<QualificationLevel>
		{
			QualificationLevel.FourToEight, QualificationLevel.OneAndTwo
		} };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));

		var model = CreateModel(session);

		var result = model.OnGet();

		Assert.IsType<PageResult>(result);
		Assert.Contains(QualificationLevel.FourToEight, model.SelectedQualificationOptions);
		Assert.Contains(QualificationLevel.OneAndTwo, model.SelectedQualificationOptions);
	}

	[Fact]
	public void OnPost_Returns_Page_When_No_Qualification_Selection()
	{
		var search = new Search();
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

		var model = CreateModel(session);
		model.SelectedQualificationOptions = new List<QualificationLevel>();

		var result = model.OnPost();

		var redirect = Assert.IsType<RedirectToPageResult>(result);
		Assert.Null(redirect.PageName);
		Assert.Equal(SharedStrings.SelectQualificationLevel, model.ValidationError);
	}

	[Fact]
	public void OnPost_Redirects_To_Age_When_Selects_QualificationNone_And_Not_VisitedCheckAnswers()
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
	public void OnPost_Redirects_To_CheckAnswers_When_Selects_None_And_VisitedCheckAnswers_And_AgeGroupSet()
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
	public void OnPost_Clears_AgeGroup_And_Redirects_CheckAnswers_When_Selects_Other_And_VisitedCheckAnswers_True()
	{
		var search = new Search { VisitedCheckAnswers = true, AgeGroup = AgeGroup.Eighteen };
		var session = new TestSession();
		session.Set("Search", Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(search)));

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