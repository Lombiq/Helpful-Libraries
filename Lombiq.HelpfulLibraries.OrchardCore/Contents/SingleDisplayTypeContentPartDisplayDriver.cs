using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.DisplayManagement.Zones;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

/// <summary>
/// A driver only used for displaying the content part in <see cref="DisplayType"/> mode. This lets the developer
/// include an overriding content part template, that won't also render the individual contained fields as separate
/// shapes. This is similar to the behavior you get when the content part is not registered with
/// <c>services.AddContentPart&lt;TPart&gt;();</c>.
/// </summary>
/// <typeparam name="TPart">The <see cref="ContentPart"/> to be displayed.</typeparam>
public abstract class SingleDisplayTypeContentPartDisplayDriver<TPart> : ContentPartDisplayDriver<TPart>
    where TPart : ContentPart, new()
{
    public abstract string DisplayType { get; }

    public virtual string Location => CommonLocationNames.Content;
    public virtual string Position => null;

    public override IDisplayResult Display(TPart part, BuildPartDisplayContext context)
    {
        var partTypeName = context.TypePartDefinition.PartDefinition.Name;
        var longShapeType = $"{partTypeName}_{context.DisplayType}";
        var shapeType = context.DisplayType != CommonContentDisplayTypes.Detail ? longShapeType : partTypeName;

        var shapeResult = new ShapeResult(shapeType, shapeContext => shapeContext.ShapeFactory.CreateAsync(
            shapeType, () => new ValueTask<IShape>(new ZoneHolding(() => shapeContext.ShapeFactory.CreateAsync("Zone"))
            {
                Properties =
                {
                    [partTypeName] = part.Content,
                    ["Part"] = part,
                    [nameof(part.ContentItem)] = part.ContentItem,
                },
            })));

        return shapeResult
            .Location(DisplayType, Position == null ? Location : $"{Location}:{Position}")
            .Displaying(displayContext =>
            {
                // Provide fully qualified alternate for Detail display type.
                if (shapeType != longShapeType) displayContext.Shape.Metadata.Alternates.Add(longShapeType);
            });
    }

    public class FieldHiderPlacementInfoResolver<TDriver> : IPlacementInfoResolver, IShapePlacementProvider
        where TDriver : SingleDisplayTypeContentPartDisplayDriver<TPart>
    {
        private readonly TDriver _driver;

        public FieldHiderPlacementInfoResolver(IServiceProvider provider) =>
            _driver = (TDriver)provider.GetRequiredService(typeof(TDriver));

        public PlacementInfo ResolvePlacement(ShapePlacementContext placementContext)
        {
            if (placementContext.DisplayType == _driver.DisplayType &&
                placementContext.Differentiator?.StartsWithOrdinal($"{typeof(TPart).Name}-") == true)
            {
                return new PlacementInfo { Location = "-" };
            }

            return null;
        }

        public Task<IPlacementInfoResolver> BuildPlacementInfoResolverAsync(IBuildShapeContext context) =>
            Task.FromResult<IPlacementInfoResolver>(this);
    }
}

public class DetailOnlyContentPartDisplayDriver<TPart> : SingleDisplayTypeContentPartDisplayDriver<TPart>
    where TPart : ContentPart, new()
{
    public override string DisplayType => CommonContentDisplayTypes.Detail;
}

public class SummaryOnlyContentPartDisplayDriver<TPart> : SingleDisplayTypeContentPartDisplayDriver<TPart>
    where TPart : ContentPart, new()
{
    public override string DisplayType => CommonContentDisplayTypes.Summary;
}
