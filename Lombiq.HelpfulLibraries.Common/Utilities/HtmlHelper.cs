using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Linq;

namespace Lombiq.HelpfulLibraries.Common.Utilities;

public static class HtmlHelper
{
    /// <summary>
    /// Parses the provided <paramref name="htmlFragment"/> as the HTML content of a single parent element.
    /// </summary>
    public static INode ParseHtmlFragment(string? htmlFragment) =>
        new HtmlParser()
            .ParseFragment($"<div>{htmlFragment}</div>", contextElement: null!)
            .Single();

    /// <summary>
    /// Extracts the inner plain text content from the provided <paramref name="htmlFragment"/>.
    /// </summary>
    /// <returns>The human-readable text content, trimmed of surrounding spaces and duplicate line breaks.</returns>
    public static string ConvertToPlainText(string? htmlFragment) =>
        ParseHtmlFragment(htmlFragment)
            .Text()
            .RegexReplace(@"\n(\s*\n)+", "\n")
            .Trim();
}
