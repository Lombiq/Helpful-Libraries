using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

/// <summary>
/// A driver only used for displaying the content part in <see cref="DisplayType"/> mode.
/// </summary>
/// <typeparam name="TPart">The <see cref="ContentPart"/> to be displayed.</typeparam>
public abstract class SingleDisplayTypeContentPartDisplayDriver<TPart> : ContentPartDisplayDriver<TPart>
    where TPart : ContentPart, new()
{
    protected abstract string DisplayType { get; }

    protected virtual string Location => CommonLocationNames.Content;
    protected virtual string Position => null;

    public override IDisplayResult Display(TPart part, BuildPartDisplayContext context) =>
        Initialize<ViewModel>(GetDisplayShapeType(context), viewModel =>
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
