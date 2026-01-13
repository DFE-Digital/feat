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

public class InterestsPageTests
{
    private static InterestsModel CreateModel(ISession session)
    {
        var httpContext = new DefaultHttpContext
        {
            Session = session
        };

        var model = new InterestsModel(NullLogger<InterestsModel>.Instance)
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
    public void OnGet_Redirects_To_Index_When_Search_Not_Updated()
    {
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new Search { Updated = false })));

        var model = CreateModel(session);

        var result = model.OnGet();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(PageName.Index, redirect.PageName);
    }

    [Fact]
    public void OnGet_Populates_Fields_From_Session_And_Returns_Page()
    {
        var search = new Search
        {
            Updated = true, 
            Location = "Sheffield", 
            Distance = Distance.Ten, 
            Interests = new List<string> { "Maths", "Fermat's last theorem" }
        };
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);

        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        Assert.Equal("Maths", model.UserInterest1);
        Assert.Equal("Fermat's last theorem", model.UserInterest2);
        Assert.Null(model.UserInterest3);
        Assert.False(model.FirstOptionMandatory);
    }

    [Theory]
    [InlineData(null, Distance.Ten)]
    [InlineData("Sheffield", null)]
    [InlineData("Sheffield", Distance.ThirtyPlus)]
    public void OnGet_Sets_FirstOptionMandatory_When_LocationOrDistance_Require_FirstOption(string? location, Distance? distance)
    {
        var search = new Search { Updated = true, Location = location, Distance = distance };
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);

        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        Assert.True(model.FirstOptionMandatory);
    }

    [Fact]
    public void OnPost_Returns_Page_When_FirstOptionMandatory_And_UserInterest1_Empty()
    {
        var search = new Search { Location = "Sheffield", Distance = Distance.ThirtyPlus }; // forces FirstOptionMandatory
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);
        model.UserInterest1 = " "; // Blank-white-space

        var result = model.OnPost();

        Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
        Assert.True(model.ModelState.ContainsKey("UserInterest1"));
        
        var errorMessage = model.ModelState["UserInterest1"]?.Errors.FirstOrDefault()?.ErrorMessage;
        
        Assert.True(SharedStrings.EnterAnInterest.Equals(errorMessage), $"Error message should read: {SharedStrings.EnterAnInterest}");
    }

    [Fact]
    public void OnPost_Saves_Interests_And_Redirects_To_QualificationLevel_When_VisitedCheckAnswers_False()
    {
        var search = new Search { VisitedCheckAnswers = false };
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);
        model.UserInterest1 = " Maths ";
        model.UserInterest2 = "Science";

        var result = model.OnPost();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(PageName.QualificationLevel, redirect.PageName);

        Assert.True(session.TryGetValue("Search", out var data));
        
        var saved = JsonSerializer.Deserialize<Search>(Encoding.UTF8.GetString(data))!;
        
        Assert.Equal(2, saved.Interests.Count);
        Assert.Equal("Maths", saved.Interests[0]);
        Assert.Equal("Science", saved.Interests[1]);
        Assert.True(saved.Updated);
    }

    [Fact]
    public void OnPost_Redirects_To_CheckAnswers_When_VisitedCheckAnswers_True()
    {
        var search = new Search { VisitedCheckAnswers = true };
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);
        model.UserInterest1 = "Maths";

        var result = model.OnPost();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(PageName.CheckAnswers, redirect.PageName);
    }
}
