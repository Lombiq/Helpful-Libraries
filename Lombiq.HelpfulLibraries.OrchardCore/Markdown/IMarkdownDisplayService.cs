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
    public static Task<string> ToHtmlAsync(
        IMarkdownDisplayService service,
        string markdown,
        MarkdownBodyPartSettings settings) =>
        service.ToHtmlAsync(markdown, settings.SanitizeHtml);
}
