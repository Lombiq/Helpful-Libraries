using OrchardCore.Media.Fields;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Lombiq.HelpfulLibraries.Libraries.Fields;

public static class MediaFieldExtensions
{
    /// <summary>
    /// Returns a collection of tuples from the provided <paramref name="mediaField"/>. Each contains the path from <see
    /// cref="MediaField.Paths"/> and the matching alt text from <see cref="MediaField.MediaTexts"/> if there is any. If
    /// there is none, the path is used as the text. This is identical behavior to the <see cref="MediaField"/>'s
    /// default display.
    /// </summary>
    public static IEnumerable<(string Path, string Text)> GetPathsAndAltTexts(this MediaField mediaField) =>
        GetPathsAndAltTexts(mediaField, format: "{0}");

    /// <summary>
    /// Returns a collection of tuples from the provided <paramref name="mediaField"/>. Each contains the path from <see
    /// cref="MediaField.Paths"/> and the matching alt text from <see cref="MediaField.MediaTexts"/> if there is any. If
    /// there is none or if it's empty a new text is generated using <paramref name="format"/>.
    /// </summary>
    /// <param name="format">
    /// When the path doesn't have a matching alt text, this string is used as the format template to generate one. The
    /// first parameter is the path, the second is just the file name.
    /// </param>
    public static IEnumerable<(string Path, string Text)> GetPathsAndAltTexts(
        this MediaField mediaField,
        string format) =>
        Enumerable.Range(0, mediaField.Paths?.Length ?? 0)
            .Select(i => (Path: mediaField.Paths[i], Text: mediaField.MediaTexts.ElementAtOrDefault(i)))
            .Select((path, text) => (
                Path: path,
                Text: string.IsNullOrWhiteSpace(text)
                    ? string.Format(CultureInfo.InvariantCulture, format, text, Path.GetFileName(text))
                    : text));
}
