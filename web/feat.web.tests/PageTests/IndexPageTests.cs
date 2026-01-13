using System.Text;
using System.Text.Json;
using feat.web.Models;
using feat.web.Pages;
using feat.web.Services;
using feat.web.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging.Abstractions;

namespace feat.web.tests.PageTests;

public class IndexPageTests
{
    private class TestHttpContextAccessor(HttpContext context) : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; } = context;
    }

    private static IndexModel CreateModel(ISession session)
    {
        var httpContext = new DefaultHttpContext
        {
            Session = session
        };

        var accessor = new TestHttpContextAccessor(httpContext);
        var staticNav = new StaticNavigationHandler(accessor);

        var model = new IndexModel(staticNav, NullLogger<IndexModel>.Instance)
        {
            PageContext = new PageContext
            {
                HttpContext = httpContext
            }
        };
        return model;
    }

    [Fact]
    public void OnGet_Sets_Search_In_Session_And_Sets_PageHistory()
    {
        //arrange
        var session = new TestSession();
        var model = CreateModel(session);

        //act
        var result = model.OnGet();

        //assert
        Assert.IsType<PageResult>(result);
        Assert.True(session.TryGetValue("Search", out var data));
        
        var saved = JsonSerializer.Deserialize<Search>(Encoding.UTF8.GetString(data))!;
        Assert.Contains(PageName.Index, saved.History);
    }
}
