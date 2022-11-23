using OrchardCore.Infrastructure.Html;
using OrchardCore.Markdown.Services;
using OrchardCore.Shortcodes.Services;
using Shortcodes;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Markdown;

public class MarkdownDisplayService : IMarkdownDisplayService
{
    private readonly IHtmlSanitizerService _htmlSanitizerService;
    private readonly IMarkdownService _markdownService;
    private readonly IShortcodeService _shortcodeService;
    public MarkdownDisplayService(
        IHtmlSanitizerService htmlSanitizerService,
        IMarkdownService markdownService,
        IShortcodeService shortcodeService)
    {
        _htmlSanitizerService = htmlSanitizerService;
        _markdownService = markdownService;
        _shortcodeService = shortcodeService;

    }

    public async Task<string> ToHtmlAsync(
        string markdown,
        bool sanitizeHtml = true,
        bool processShortcodes = true,
        Context shortcodesContext = null)
    {
        if (string.IsNullOrWhiteSpace(markdown)) return string.Empty;

        var html = _markdownService.ToHtml(markdown);

        // Process shortcodes if desired and if it could contain a shortcode.
        if (processShortcodes && html.Contains('['))
        {
            html = await _shortcodeService.ProcessAsync(html, shortcodesContext ?? new Context());
        }

        return sanitizeHtml
            ? _htmlSanitizerService.Sanitize(html ?? string.Empty)
            : html;

    }
}
