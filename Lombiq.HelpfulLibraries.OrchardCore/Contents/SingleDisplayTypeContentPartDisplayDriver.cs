using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
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
    protected abstract string DisplayType { get; }

    protected virtual string Location => CommonLocationNames.Content;
    protected virtual string Position => null;

    public override IDisplayResult Display(TPart part, BuildPartDisplayContext context) =>
        new OverridingShapeResult(context, part)
            .Initialize(viewModel =>
            {
                viewModel.Content = part.Content;
                viewModel.ContentItem = part.ContentItem;
                viewModel.Part = part;
            })
            .Location(DisplayType, Position == null ? Location : $"{Location}:{Position}");

    public class ViewModel
    {
        public dynamic Content { get; set; }
        public ContentItem ContentItem { get; set; }
        public ContentPart Part { get; set; }
    }

    public class OverridingShapeResult : IDisplayResult
    {
        private readonly BuildPartDisplayContext _buildPartDisplayContext;
        private readonly ContentPart _contentPart;

        private Action<ViewModel> _initializeAction;
        private string _displayType;
        private string _location;

        public OverridingShapeResult(
            BuildPartDisplayContext buildPartDisplayContext,
            ContentPart contentPart)
        {
            _buildPartDisplayContext = buildPartDisplayContext;
            _contentPart = contentPart;
        }

        public OverridingShapeResult Initialize(Action<ViewModel> action)
        {
            _initializeAction = action;
            return this;
        }

        public OverridingShapeResult Location(string displayType, string locationDescription)
        {
            _displayType = displayType;
            _location = locationDescription;
            return this;
        }

        public async Task ApplyAsync(BuildDisplayContext context)
        {
            var contentTypePartDefinition = _buildPartDisplayContext.TypePartDefinition;
            var contentType = _contentPart.ContentItem.ContentType;
            var partName = contentTypePartDefinition.Name;
            var partTypeName = contentTypePartDefinition.PartDefinition.Name;
            var hasStereotype = contentTypePartDefinition.ContentTypeDefinition.TryGetStereotype(out var stereotype);

            var shapeType = context.DisplayType != "Detail" ? "ContentPart_" + context.DisplayType : "ContentPart";

            var shapeResult = new ShapeResult(
                shapeType,
                shapeContext => shapeContext.ShapeFactory.CreateAsync(shapeType, _initializeAction));
            shapeResult.Differentiator(partName);
            shapeResult.Name(partName);
            shapeResult.Location(_displayType, _location);
            shapeResult.OnGroup(context.GroupId);
            shapeResult.Displaying(ctx =>
            {
                var displayTypes = new[]
                {
                    string.Empty,
                    "_" + ctx.Shape.Metadata.DisplayType,
                };

                foreach (var displayType in displayTypes)
                {
                    // eg. ServicePart,  ServicePart.Summary
                    ctx.Shape.Metadata.Alternates.Add($"{partTypeName}{displayType}");

                    // [ContentType]_[DisplayType]__[PartType]
                    // e.g. LandingPage-ServicePart, LandingPage-ServicePart.Summary
                    ctx.Shape.Metadata.Alternates.Add($"{contentType}{displayType}__{partTypeName}");

                    if (hasStereotype)
                    {
                        // [Stereotype]_[DisplayType]__[PartType],
                        // e.g. Widget-ServicePart
                        ctx.Shape.Metadata.Alternates.Add($"{stereotype}{displayType}__{partTypeName}");
                    }
                }

                if (partTypeName == partName)
                {
                    return;
                }

                foreach (var displayType in displayTypes)
                {
                    // [ContentType]_[DisplayType]__[PartName]
                    // e.g. Employee-Address1, Employee-Address2
                    ctx.Shape.Metadata.Alternates.Add($"{contentType}{displayType}__{partName}");

                    if (hasStereotype)
                    {
                        // [Stereotype]_[DisplayType]__[PartType]__[PartName]
                        // e.g. Widget-Services
                        ctx.Shape.Metadata.Alternates.Add($"{stereotype}{displayType}__{partTypeName}__{partName}");
                    }
                }
            });

            await shapeResult.ApplyAsync(context);

            // Replace the context's shape with the newly generated one.
            typeof(BuildShapeContext)
                .GetProperty(nameof(BuildShapeContext.Shape))!
                .GetSetMethod(nonPublic: true)!
                .Invoke(context, new object[] { shapeResult.Shape });
        }

        public Task ApplyAsync(BuildEditorContext context) => Task.CompletedTask;
    }
}

public class DetailOnlyContentPartDisplayDriver<TPart> : SingleDisplayTypeContentPartDisplayDriver<TPart>
    where TPart : ContentPart, new()
{
    protected override string DisplayType => CommonContentDisplayTypes.Detail;
}

public class SummaryOnlyContentPartDisplayDriver<TPart> : SingleDisplayTypeContentPartDisplayDriver<TPart>
    where TPart : ContentPart, new()
{
    protected override string DisplayType => CommonContentDisplayTypes.Summary;
}
