using System.Text.RegularExpressions;
using UnDotNet.HtmlToText;

namespace feat.common.Extensions;

public static class StringExtensions
{
    private static readonly HtmlToTextConverter Converter = new();
    
    extension(string? source)
    {
        public string Scrub()
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
   
        public string? CleanHtml()
        {
            // If our string is null, empty, return the string
            return string.IsNullOrEmpty(source) ? source : Converter.Convert(source);
        }

        public string ValueOrNotProvided()
        {
            return string.IsNullOrEmpty(source) ? SharedStrings.NotProvided : source;
        }

        public string TruncateString(out string remainder, int cutoffLength = 300)
        {
            {
                remainder = string.Empty;

                if (string.IsNullOrEmpty(source) || cutoffLength <= 0)
                {
                    remainder = source ?? string.Empty;
                    return string.Empty;
                }

                if (source.Length <= cutoffLength)
                {
                    remainder = string.Empty;
                    return source;
                }

                // Sentence-ending punctuation to look for
                char[] sentenceEndings = ['.', '!', '?'];

                // Define lower boundary where we still accept punctuation
                var punctuationMin = cutoffLength - 50;  // mirrors 300-50 = 250 logic

                // Find last sentence-ending punctuation before cutoff
                var lastSentenceEnd = sentenceEndings
                    .Select(end => source.LastIndexOf(end, cutoffLength - 1))
                    .Prepend(-1)
                    .Max();

                int cutIndex;

                if (lastSentenceEnd >= punctuationMin)
                {
                    // Cut at punctuation
                    cutIndex = lastSentenceEnd + 1;
                }
                else
                {
                    // Otherwise cut at last space
                    var lastSpace = source.LastIndexOf(' ', cutoffLength - 1);
                    cutIndex = lastSpace > -1 ? lastSpace : cutoffLength;
                }

                // Compose return values
                var result = source.Substring(0, cutIndex);
                remainder = source.Substring(cutIndex).TrimStart();

                return result;
            }
        }
    }
    
    private static string CheckForWebsite(string source)
    {
        var regex = new Regex(
            @"https\:\/\/|web\s*site.*details|see (course |school |provider )*web\s*site|web\s*site.*information|web\s*site for (more|the|entry requirements|course booket)|please see web|refer to our web|(our|course|provider|school|college|ocr|edexcel|aqa|Eduqas|examination board)+ web\s*site|www\.|of the website",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        return regex.IsMatch(source) ? string.Empty : source;
    }
}