using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using System;
using System.Threading.Tasks;

namespace OrchardCore.DisplayManagement.Views;

/// <summary>
/// Helper class for generating placement strings.
/// </summary>
public static class ShapeResultExtensions
{
    /// <summary>
    /// Uses <see cref="ShapeResult.Location(string)"/> to set the <paramref name="name"/> of the tab and its <paramref
    /// name="priority"/> in the order of the tabs.
    /// </summary>
    public static ShapeResult UseTab(
        this ShapeResult shapeResult,
        string name,
        double priority,
        string placement = CommonLocationNames.Parts) =>
        shapeResult.Location(FormattableString.Invariant($"{placement}#{name}:{priority}"));

    /// <summary>
    /// The shape will only be rendered if <paramref name="condition"/> is <see langword="true"/>. This is a shortcut
    /// for <see cref="ShapeResult.RenderWhen"/> in case the condition is known before the shape is initialized.
    /// </summary>
    public static ShapeResult RenderWhen(this ShapeResult shapeResult, bool condition) =>
        shapeResult.RenderWhen(() => Task.FromResult(condition));

    /// <summary>
    /// Sets the <see cref="ShapeResult"/>'s location to a local zone with optional sorting information.
    /// </summary>
    public static ShapeResult PlaceInZone(this ShapeResult shapeResult, string zoneName, double? priority = null) =>
        shapeResult.Location(priority is { } number
            ? FormattableString.Invariant($"{zoneName}:{number}")
            : zoneName);

    /// <summary>
    /// Sets the <see cref="ShapeResult"/>'s location to a global layout zone with optional sorting information.
    /// </summary>
    public static ShapeResult PlaceInGlobalZone(
        this ShapeResult shapeResult,
        string globalZone,
        double? priority = null) =>
        shapeResult.PlaceInZone("/" + globalZone, priority);

    /// <summary>
    /// Sets the <see cref="ShapeResult"/>'s location to <see cref="CommonLocationNames.Content"/> with optional sorting
    /// information.
    /// </summary>
    public static ShapeResult PlaceInContent(this ShapeResult shapeResult, double? priority = null) =>
        shapeResult.PlaceInZone(CommonLocationNames.Content, priority);
}
