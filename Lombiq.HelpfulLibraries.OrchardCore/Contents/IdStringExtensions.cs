using System;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

public static class IdStringExtensions
{
    /// <summary>
    /// Converts a string into a still valid Content Item ID by turning it into lower-case, removing any non-
    /// alphanumeric characters, and finally padding the rest with zeroes so it has the expected length of 26
    /// characters. This way <c>~Help: Popular Topics</c> can be used to get the ID <c>helppopulartopics000000000</c>,
    /// which can be used in recipes without complication. (The example's leading tilde makes it easier to identify as
    /// a potential ID even if it's not immediately passed to this method.)
    /// </summary>
    /// <param name="text">The arbitrary string to convert. May include spaces, punctuation, etc.</param>
    /// <returns>The valid 26 character long lower-case ContentItemId.</returns>
    public static string ToContentItemId(this string text)
    {
        // We use lower case because that's how all ContentItem IDs are cased.
#pragma warning disable CA1308
        var contentItemId = text
            .ToLowerInvariant()
            .RegexReplace(@"[^a-z0-9]+", string.Empty);
#pragma warning restore CA1308

        if (contentItemId.Length < 26) contentItemId = contentItemId.PadRight(26, '0');

        return contentItemId;
    }
}
