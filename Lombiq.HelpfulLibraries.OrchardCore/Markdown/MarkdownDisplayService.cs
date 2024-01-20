using OrchardCore.Infrastructure.Html;
using OrchardCore.Markdown.Services;
using OrchardCore.Shortcodes.Services;
using Shortcodes;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Markdown;

public class MarkdownDisplayService(
    IHtmlSanitizerService htmlSanitizerService,
    IMarkdownService markdownService,
    IShortcodeService shortcodeService) : IMarkdownDisplayService
{
    public async Task<string> ToHtmlAsync(
        string markdown,
        bool sanitizeHtml = true,
        bool processShortcodes = true,
        Context shortcodesContext = null)
    {
        if (string.IsNullOrWhiteSpace(markdown)) return string.Empty;

        var html = markdownService.ToHtml(markdown);

        // Process shortcodes if desired and if it could contain a shortcode.
        if (processShortcodes && html.Contains('['))
        {
            html = await shortcodeService.ProcessAsync(html, shortcodesContext ?? new Context());
        }

        return sanitizeHtml
            ? htmlSanitizerService.Sanitize(html ?? string.Empty)
            : html;
    }
}
