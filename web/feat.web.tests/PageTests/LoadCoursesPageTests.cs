using System.Text;
using System.Text.Json;
using feat.web.Enums;
using feat.web.Models;
using feat.web.Models.ViewModels;
using feat.web.Pages;
using feat.web.Services;
using feat.web.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Facet = feat.web.Models.ViewModels.Facet;

namespace feat.web.tests.PageTests;

public class LoadCoursesPageTests
{
    private static LoadCoursesModel CreateModel(ISession session, ISearchService? searchService = null)
    {
        var httpContext = new DefaultHttpContext
        {
            Session = session
        };

        searchService ??= Substitute.For<ISearchService>();

        return new LoadCoursesModel(searchService, NullLogger<LoadCoursesModel>.Instance)
        {
            PageContext = new PageContext { HttpContext = httpContext },
            Search = new Search { Updated = true } 
        };
    }

    [Fact]
    public async Task OnGetAsync_Redirects_To_Index_When_Search_NotUpdated()
    {
        var session = new TestSession();
        var search = new Search { Updated = false };
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));
        var model = CreateModel(session);
        
        var result = await model.OnGetAsync(null!);
        
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(PageName.Index, redirect.PageName);
    }

    [Fact]
    public async Task OnGetAsync_Populates_Courses_And_Facets_From_Service()
    {
        var session = new TestSession();
        var search = new Search { Updated = true };
        
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));
        
        session.Set("AllFacets", "[]"u8.ToArray());

        var courses = new List<Course>
        {
            new() { Id = Guid.NewGuid(), Title = "Course 1", InstanceId = Guid.NewGuid() }
        };
        
        var facets = new List<Models.Facet>
        {
            new()
            { 
                Name = "LearningMethod", 
                Values = new Dictionary<string, long> { { "Online", 1 } } 
            }
        };

        var searchService = Substitute.For<ISearchService>();
        searchService.Search(Arg.Any<Search>(), Arg.Any<string>())
            .Returns(new SearchResponse
            {
                Courses = courses,
                TotalCount = 1,
                Page = 1,
                PageSize = 10,
                Facets = facets
            });

        var model = CreateModel(session, searchService);
        
        var result = await model.OnGetAsync(null!);
        
        Assert.IsType<PageResult>(result);
        Assert.Single(model.Courses);
        Assert.NotEmpty(model.AllFacets);
    }

    [Fact]
    public void OnPostUpdateSelection_Updates_Search_And_Facets()
    {
        var session = new TestSession();
        var search = new Search { Updated = true };
        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));
        
        var existingFacets = new List<Facet>
        {
            new() {
                Name = "QualificationLevel",
                DisplayName = "Qualification Level",
                Values =
                [
                    new FacetValue
                    {
                        Name = "Level1",
                        DisplayName = "Level 1 (like first certificate)",
                        Selected = false,
                        
                    },

                    new FacetValue
                    {
                        Name = "Level2",
                        DisplayName = "Level 2 (like GCSEs)",
                        Selected = false
                    }
                ]
            }
        };
        session.Set("AllFacets", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(existingFacets)));

        var model = CreateModel(session);
        model.SelectedTravelDistance = Distance.Ten;
        
        model.AllFacets =
        [
            new Facet
            {
                Name = "QualificationLevel",
                DisplayName = "Qualification Level",
                Values =
                [
                    new FacetValue
                    {
                        Name = "Level1",
                        DisplayName = "Level 1 (like first certificate)",
                        Selected = true
                    }
                ]
            }
        ];
        
        var result = model.OnPostUpdateSelection();
        
        Assert.IsType<RedirectToPageResult>(result);
        
        var savedSearch = JsonSerializer.Deserialize<Search>(Encoding.UTF8.GetString(session.Get("Search")!))!;
        Assert.Equal(Distance.Ten, savedSearch.Distance);
        
        var savedFacets = JsonSerializer.Deserialize<List<Facet>>(Encoding.UTF8.GetString(session.Get("AllFacets")!))!;
        Assert.True(savedFacets[0].Values.First(v => v.Name == "Level1").Selected);
    }

    [Fact]
    public void OnGetClearFilters_Resets_Facets_And_Search()
    {
        var session = new TestSession();
        var search = new Search { Updated = true, Distance = Distance.Ten, OriginalDistance = Distance.Five };
        var facet = new Facet { Name = "Test", DisplayName = "Test", Values = new List<FacetValue>() };

        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));
        session.Set("AllFacets", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new List<Facet> { facet })));

        var model = CreateModel(session);
        
        var result = model.OnGetClearFilters();
        
        Assert.IsType<RedirectToPageResult>(result);
        
        Assert.False(session.TryGetValue("AllFacets", out _));
        
        var savedSearch = JsonSerializer.Deserialize<Search>(Encoding.UTF8.GetString(session.Get("Search")!))!;
        Assert.Equal(Distance.Five, savedSearch.Distance);
        Assert.Empty(savedSearch.Facets);
    }

    [Fact]
    public void OnGetClearFacet_Unselects_Facet_Values()
    {
        var session = new TestSession();
        var search = new Search { Updated = true };
        var facetName = "QualificationLevel";
        var facets = new List<Facet>
        {
            new() {
                Name = facetName,
                DisplayName = "Qualification Level",
                Values =
                [
                    new FacetValue
                    {
                        Name = "Level1",
                        DisplayName = "Level 1 (like first certificate)",
                        Selected = true
                    },

                    new FacetValue
                    {
                        Name = "Level2",
                        DisplayName = "Level 2 (like GCSEs)",
                        Selected = true
                    }
                ]
            }
        };

        session.Set("Search", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(search)));
        session.Set("AllFacets", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(facets)));

        var model = CreateModel(session);
        
        var result = model.OnGetClearFacet(facetName);
        
        Assert.IsType<RedirectToPageResult>(result);

        var savedFacets = JsonSerializer.Deserialize<List<Facet>>(Encoding.UTF8.GetString(session.Get("AllFacets")!))!;
        Assert.All(savedFacets[0].Values, v => Assert.False(v.Selected));
    }
}