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
    private static (QualificationLevelModel model, TestSession session) CreateModelWithSearch(Search? search = null)
    {
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search ?? new Search { Updated = true })));

        var model = new QualificationLevelModel(NullLogger<QualificationLevelModel>.Instance)
        {
            PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Session = session
                }
            },
            Search = null!
        };

        return (model, session);
    }

    [Fact]
    public void OnGet_Populates_SelectedQualificationOptions_From_Session()
    {
        var search = new Search
        {
            Updated = true,
            QualificationLevels = [QualificationLevel.FourToEight, QualificationLevel.OneAndTwo]
        };
        var (model, _) = CreateModelWithSearch(search);

        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        Assert.Contains(QualificationLevel.FourToEight, model.SelectedQualificationOptions);
        Assert.Contains(QualificationLevel.OneAndTwo, model.SelectedQualificationOptions);
    }

    [Fact]
    public void OnGet_Populates_SelectedQualificationOptions_From_TempData()
    {
        var (model, _) = CreateModelWithSearch();
        model.SelectedQualificationsJson = JsonSerializer.Serialize(new List<QualificationLevel> { QualificationLevel.Three });

        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        Assert.Contains(QualificationLevel.Three, model.SelectedQualificationOptions);
    }

    [Fact]
    public void OnGet_Adds_ModelState_Error_From_ValidationError()
    {
        var (model, _) = CreateModelWithSearch();
        model.ValidationError = "Custom error";

        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
        Assert.True(model.ModelState.ContainsKey(nameof(model.SelectedQualificationOptions)));
    }

    [Fact]
    public void OnPost_Returns_Page_When_No_Qualification_Selected()
    {
        var (model, _) = CreateModelWithSearch();
        model.SelectedQualificationOptions = new List<QualificationLevel>();

        var result = model.OnPost();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Null(redirect.PageName);
        Assert.Equal(SharedStrings.SelectQualificationLevel, model.ValidationError);
    }

    [Theory]
    [InlineData(QualificationLevel.None, PageName.Age, false)]
    [InlineData(QualificationLevel.None, PageName.CheckAnswers, true)]
    [InlineData(QualificationLevel.OneAndTwo, PageName.Age, false)]
    [InlineData(QualificationLevel.OneAndTwo, PageName.CheckAnswers, true)]
    public void OnPost_Redirects_Correctly_Based_On_Selection(QualificationLevel level, string expectedPage, bool visitedCheckAnswers)
    {
        var search = new Search
        {
            VisitedCheckAnswers = visitedCheckAnswers,
            AgeGroup = visitedCheckAnswers ? AgeGroup.Eighteen : null
        };
        var (model, session) = CreateModelWithSearch(search);

        model.SelectedQualificationOptions = [level];

        var result = model.OnPost();
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(expectedPage, redirect.PageName);

        Assert.True(session.TryGetValue("Search", out var data));
        var saved = JsonSerializer.Deserialize<Search>(Encoding.UTF8.GetString(data))!;
        Assert.Contains(level, saved.QualificationLevels);
        Assert.True(saved.Updated);
    }

    [Fact]
    public void OnPost_Clears_AgeGroup_And_Redirects_CheckAnswers_When_Selects_Other_And_VisitedCheckAnswers()
    {
        var search = new Search { VisitedCheckAnswers = true, AgeGroup = AgeGroup.Eighteen };
        var (model, session) = CreateModelWithSearch(search);

        model.SelectedQualificationOptions = [QualificationLevel.FourToEight];

        var result = model.OnPost();
        var redirect = Assert.IsType<RedirectToPageResult>(result);

        Assert.Equal(PageName.CheckAnswers, redirect.PageName);

        Assert.True(session.TryGetValue("Search", out var data));
        var saved = JsonSerializer.Deserialize<Search>(Encoding.UTF8.GetString(data))!;
        Assert.Null(saved.AgeGroup);
        Assert.True(saved.Updated);
    }
}