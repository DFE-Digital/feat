using System.Globalization;

namespace feat.web.Models.ViewModels;

public record StartDate(DateTime Date)
{
    private static readonly CultureInfo DisplayCulture = new("en-GB");

    public string Display => Date.ToString("d MMMM yyyy", DisplayCulture);
}
