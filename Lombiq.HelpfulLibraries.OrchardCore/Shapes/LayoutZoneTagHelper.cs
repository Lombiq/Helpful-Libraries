using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.DisplayManagement.Shapes;
using OrchardCore.DisplayManagement.Zones;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Shapes;

/// <summary>
/// A tag helper similar to &lt;zone&gt;, but it works even when it's used in Layout.cshtml (prior to its corresponding
/// <c>RenderSectionAsync</c>) when the same zone contains widgets (for efforts towards updating &lt;zone&gt; the same
/// way see https://github.com/OrchardCMS/OrchardCore/issues/11481).
/// </summary>
[HtmlTargetElement("layout-zone", Attributes = NameAttribute)]
public sealed class LayoutZoneTagHelper : TagHelper
{
    private const string PositionAttribute = "position";
    private const string NameAttribute = "name";

    private readonly ILayoutAccessor _layoutAccessor;
    private readonly Func<ValueTask<IShape>> _zoneFactory;
    private readonly ILogger<LayoutZoneTagHelper> _logger;

    public LayoutZoneTagHelper(
        ILayoutAccessor layoutAccessor,
        IShapeFactory shapeFactory,
        ILogger<LayoutZoneTagHelper> logger)
    {
        _layoutAccessor = layoutAccessor;
        _zoneFactory = () => shapeFactory.CreateAsync("Zone");
        _logger = logger;
    }

    [HtmlAttributeName(PositionAttribute)]
    public string Position { get; set; }

    [HtmlAttributeName(NameAttribute)]
    public string Name { get; set; }

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (string.IsNullOrEmpty(Name))
        {
            throw new ArgumentException("The name attribute can't be empty");
        }

        return ProcessInnerAsync(output);
    }

    private async Task ProcessInnerAsync(TagHelperOutput output)
    {
        var childContent = await output.GetChildContentAsync();
        var layout = await _layoutAccessor.GetLayoutAsync();

        var zone = layout.Zones[Name];

        if (zone is PositionWrapper positionWrapper && layout is ZoneHolding zoneHolding)
        {
            zone = new ZoneOnDemand(_zoneFactory, zoneHolding, Name);
            await zone.AddAsync(positionWrapper, positionWrapper.Position);
        }

        if (zone is Shape shape)
        {
            await shape.AddAsync(childContent, Position);
        }
        else
        {
            _logger.LogWarning(
                "Unable to add shape to the zone using the <zone2> tag helper because the zone's type is " +
                "\"{ActualType}\" instead of the expected {ExpectedType}",
                zone.GetType().FullName,
                nameof(Shape));
        }

        // Don't render the zone tag or the inner content
        output.SuppressOutput();
    }
}
