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

public class AgePageTests
{
    private static AgeModel CreateModel(ISession session)
    {
        var httpContext = new DefaultHttpContext
        {
            Session = session
        };

        var model = new AgeModel(NullLogger<AgeModel>.Instance)
        {
            PageContext = new PageContext
            {
                HttpContext = httpContext
            },
            Search = new  Search(),
        };
        return model;
    }

    [Fact]
    public void OnGet_AgeGroup_In_Session_And_Returns_Page()
    {
        var search = new Search { AgeGroup = AgeGroup.TwentyToTwentyFour, Updated = true };
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);

        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        Assert.Equal(AgeGroup.TwentyToTwentyFour, model.AgeGroup);
    }

    [Fact]
    public void OnGet_No_AgeGroup_In_Session_Returns_Page()
    {
        var search = new Search { Updated = true };
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);

        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        Assert.Null(model.AgeGroup);
    }

    [Fact]
    public void OnPost_ModelState_Invalid_Returns_Page()
    {
        var search = new Search();
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);
        model.ModelState.AddModelError("AgeGroup", "error");

        var result = model.OnPost();

        Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
    }

    [Fact]
    public void OnPost_Save_AgeGroup_And_Redirects_To_CheckAnswers()
    {
        var search = new Search();
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);
        model.AgeGroup = AgeGroup.Eighteen;

        var result = model.OnPost();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(PageName.CheckAnswers, redirect.PageName);

        Assert.True(session.TryGetValue("Search", out var data));
        var saved = JsonSerializer.Deserialize<Search>(Encoding.UTF8.GetString(data))!;
        
        Assert.Equal(AgeGroup.Eighteen, saved.AgeGroup);
        Assert.True(saved.Updated);
    }

    [Fact]
    public void OnPost_AgeGroup_NotSet_Redirects_To_CheckAnswers()
    {
        var search = new Search();
        var session = new TestSession();
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));

        var model = CreateModel(session);
        model.AgeGroup = null;

        var result = model.OnPost();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(PageName.CheckAnswers, redirect.PageName);

        Assert.True(session.TryGetValue("Search", out var data));
        
        var saved = JsonSerializer.Deserialize<Search>(Encoding.UTF8.GetString(data))!;
        Assert.Null(saved.AgeGroup);
        Assert.True(saved.Updated);
    }
}
