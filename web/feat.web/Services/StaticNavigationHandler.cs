using System.Text.Json;

namespace feat.web.Services;

// Handles back-button navigation outside of the Search-Journey flow.
public class StaticNavigationHandler(IHttpContextAccessor httpContextAccessor)
{
    private const string BackPageSessionKey = "StaticNavigationBackPage2311";

    public void Initialise()
    {
        SaveBackHistory([]);
    }
    
    public string GetRefererUrl()
    {
        var values = AddToPageHistory();
        return GetPreviousPage(values.referingUrl, values.pageName);
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
            var refererLink = value.ToString();
            
            var staticPageHistory = GetBackLinkHistory();

            // Prevent duplicate consecutive entries
            if (staticPageHistory.Count != 0 && (staticPageHistory[staticPageHistory.Count - 1] == refererLink))
            {
                return (refererLink, string.Empty); 
            }

            var currentPageName = httpContext.Request.Path.ToString().Split('/')?.LastOrDefault();
            
            if (!string.IsNullOrEmpty(currentPageName) && refererLink.EndsWith(currentPageName)) 
            {
                return (staticPageHistory[^1], string.Empty);
            }
            
            if (!staticPageHistory.Contains(refererLink))
            {
                staticPageHistory.Add(refererLink);
            }
            
            SaveBackHistory(staticPageHistory);
            
            return (refererLink, currentPageName);
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

    private string GetPreviousPage(string? referUrl, string? pageName)
    {
        if (pageName == null || referUrl == null)
        {
            return string.Empty;
        }

        if (!string.IsNullOrEmpty(referUrl) && string.IsNullOrEmpty(pageName))
        {
            return referUrl;
        }

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
            var index = history.FindIndex(x => pageName != null && x.EndsWith(pageName));
            history.RemoveRange(index, history.Count - index);
            var navigatingBackUrl = history[^1];

            SaveBackHistory(history);

            return navigatingBackUrl;
        }

        var previousPage = history.Count > 1 ? history[^1] : history.FirstOrDefault();

        if (previousPage == null && !string.IsNullOrEmpty(referUrl))
        {
            return referUrl;
        }

        return previousPage ?? string.Empty;
    }

}