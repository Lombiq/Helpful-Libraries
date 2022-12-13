using OrchardCore.Markdown.Settings;
using Shortcodes;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Markdown;

/// <summary>
/// A service for displaying Markdown as HTML, similar to what MarkdownBodyPart does.
/// </summary>
public interface IMarkdownDisplayService
{
    /// <summary>
    /// Converts Markdown to HTML. Optionally sanitizes HTML and processes shortcodes.
    /// </summary>
    Task<string> ToHtmlAsync(
        string markdown,
        bool sanitizeHtml = true,
        bool processShortcodes = true,
        Context shortcodesContext = null);
}

public static class MarkdownDisplayServiceExtensions
{
    /// <summary>
    /// Using <see cref="IMarkdownDisplayService"/>, converts Markdown to HTML, processes shortcodes, and optionally
    /// sanitizes the HTML.
    /// </summary>
    /// <param name="markdown">The markdown string to sanitize.</param>
    /// <param name="settings">The settings that determine whether the HTML should be sanitized.</param>
    /// <returns>The converted HTML string.</returns>
    public static Task<string> ToHtmlAsync(
        IMarkdownDisplayService service,
        string markdown,
        MarkdownBodyPartSettings settings) =>
        service.ToHtmlAsync(markdown, settings.SanitizeHtml);
}
