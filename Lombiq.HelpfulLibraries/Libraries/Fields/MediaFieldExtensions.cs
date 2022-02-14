using OrchardCore.Media.Fields;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.HelpfulLibraries.Libraries.Fields
{
    public static class MediaFieldExtensions
    {
        /// <summary>
        /// Returns a collection of tuples from the provided <paramref name="mediaField"/>. Each contains the path from
        /// <see cref="MediaField.Paths"/> and the matching alt text from <see cref="MediaField.MediaTexts"/> if there
        /// is any. If there is none, the path is used as the text. This is identical behavior to the <see
        /// cref="MediaField"/>'s default display.
        /// </summary>
        public static IEnumerable<(string Path, string Text)> GetPathsAndAltTexts(this MediaField mediaField) =>
            Enumerable.Range(0, mediaField.Paths?.Length ?? 0)
                .Select(i => (
                    Path: mediaField.Paths[i],
                    Text: mediaField.MediaTexts.ElementAtOrDefault(i) is { } text &&
                          !string.IsNullOrWhiteSpace(text)
                        ? text
                        : mediaField.Paths[i]));
    }
}
