using System.Text.Json;
using feat.web.Models;

namespace feat.web.Services;

// Handles back-button navigation outside of the Search-Journey flow.
public class StaticNavigationHandler(IHttpContextAccessor httpContextAccessor)
{
    private const string BackPageSessionKey = "StaticNavigationBackPage23";

    public void Initialise()
    {
        SaveBackHistory([]);
    }
    
    public string GetRefererUrl()
    {
        var values = AddToPageHistory();
        return GetPreviousPage(values.pageName, values.referingUrl);
    }

    private (string? referingUrl, string? pageName) AddToPageHistory()
    {
        var httpContext = httpContextAccessor.HttpContext;
        
        var session = httpContext?.Session;
        if (session == null)
        {
            return (null, null);
        }

        if (httpContext != null &&
            httpContext.Request.Headers!.TryGetValue("Referer", out var value))
        {
            var referer = value.ToString();
            var staticPageHistory = GetBackLinkHistory();

            // Prevent duplicate consecutive entries
            if (staticPageHistory.Count != 0 && (staticPageHistory[staticPageHistory.Count - 1] == referer))
                return new ValueTuple<string?, string?>();
            
            if (!staticPageHistory.Contains(referer))
            {
                staticPageHistory.Add(referer);
            }
            SaveBackHistory(staticPageHistory);
            
            var currentPageName = httpContext.Request.Path.ToString().Split('/')?.LastOrDefault();
            
            return (referer, currentPageName);
        }
        return (null, null);
    }

    private List<string> GetBackLinkHistory()
    {
        var session = httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return [];

        var jsonUrls = session.GetString(BackPageSessionKey);

        return string.IsNullOrEmpty(jsonUrls)
            ? []
            : JsonSerializer.Deserialize<List<string>>(jsonUrls) ?? [];
    }

    private void SaveBackHistory(List<string> staticHistory)
    {
        var session = httpContextAccessor.HttpContext?.Session;
        
        session?.SetString(BackPageSessionKey, JsonSerializer.Serialize(staticHistory));
    }

    private string GetPreviousPage(string? pageName, string? referUrl)
    {
        var history = GetBackLinkHistory();
        var navigatingBack = false;

        foreach (var item in history)
        {
            if (pageName != null && item.Contains(pageName))
            {
                navigatingBack = true;
                break;
            }
        }

        if (navigatingBack)
        {
            history.RemoveRange(history.Count - 2, 2);
            var navigatingBackUrl = history[history.Count - 1];

            SaveBackHistory(history);
            
            return navigatingBackUrl;
        }

        string? previousPage = history.Count > 1 ? history[history.Count - 1] : history.FirstOrDefault();

        if (previousPage == null && !string.IsNullOrEmpty(referUrl))
        {
            return referUrl; 
        }
        
        return previousPage ?? string.Empty;
    }
    
}