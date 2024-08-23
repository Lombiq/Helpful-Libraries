using Microsoft.AspNetCore.Razor.TagHelpers;
using OrchardCore.DisplayManagement;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.TagHelpers;

/// <summary>
/// A base class for tag helpers that return a shape.
/// </summary>
public abstract class ShapeTagHelperBase<TModel> : TagHelper
{
    protected readonly IDisplayHelper _displayHelper;
    protected readonly IShapeFactory _shapeFactory;

    /// <summary>
    /// Gets the type name of the shape to be displayed. If it returns <see langword="null"/>, the then <see
    /// cref="GetShapeTypeAsync"/> is evaluated instead.
    /// </summary>
    protected abstract string ShapeType { get; }

    protected ShapeTagHelperBase(IDisplayHelper displayHelper, IShapeFactory shapeFactory)
    {
        _displayHelper = displayHelper;
        _shapeFactory = shapeFactory;
    }

    /// <summary>
    /// Returns the view-model to be used when displaying the shape.
    /// </summary>
    protected abstract ValueTask<TModel> GetViewModelAsync(TagHelperContext context, TagHelperOutput output);

    /// <summary>
    /// If <see cref="ShapeType"/> is <see langword="null"/>, this method is evaluated to calculate the shape type.
    /// </summary>
    protected virtual ValueTask<string> GetShapeTypeAsync(TagHelperContext context, TagHelperOutput output) =>
        throw new NotSupportedException(
            $"Set {nameof(ShapeType)} to not null or implement {nameof(GetShapeTypeAsync)} in the derived type.");

    /// <summary>
    /// Optional tasks once the <paramref name="output"/>'s tag is disabled and the <see
    /// cref="TagHelperOutput.PostContent"/> is already set to the rendered shape.
    /// </summary>
    protected virtual ValueTask PostProcessAsync(TagHelperContext context, TagHelperOutput output) =>
        ValueTask.CompletedTask;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var shape = await _shapeFactory.CreateAsync(
            ShapeType ?? await GetShapeTypeAsync(context, output),
            await GetViewModelAsync(context, output));
        var content = await _displayHelper.ShapeExecuteAsync(shape);

        output.TagName = null;
        output.TagMode = TagMode.StartTagAndEndTag;
        output.PostContent.SetHtmlContent(content);
        await PostProcessAsync(context, output);
    }
}
