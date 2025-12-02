using System.Text.RegularExpressions;
using UnDotNet.HtmlToText;

namespace feat.common.Extensions;

public static class StringExtensions
{
    private static HtmlToTextConverter _converter = new HtmlToTextConverter();
    
    public static string Scrub(this string? source)
    {
        if (source == null)
            return string.Empty;

        var result = source.Trim() switch
        {
            "*" or "***" or "*****" => string.Empty,
            "-" or "?" or "1" or "a" or "n" or "x" or "z" => string.Empty,
            "xx" or "xxx" => string.Empty,
            "n/a" or "TBA" or "TBC" or "PENDING" or "To follow" or "To be added" =>  string.Empty, 
            "See website" or "This course" => string.Empty,
            _ => CheckForWebsite(source.Trim())
        };
        
        return result;
    }

    private static string CheckForWebsite(string source)
    {
        var regex = new Regex(
            @"https\:\/\/|web\s*site.*details|see (course |school |provider )*web\s*site|web\s*site.*information|web\s*site for (more|the|entry requirements|course booket)|please see web|refer to our web|(our|course|provider|school|college|ocr|edexcel|aqa|Eduqas|examination board)+ web\s*site|www\.|of the website",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        return regex.IsMatch(source) ? string.Empty : source;
    }

    public static string? CleanHTML(this string? source)
    {
        // If our string is null, empty, return the string
        return string.IsNullOrEmpty(source) ? source : _converter.Convert(source);
    }
}