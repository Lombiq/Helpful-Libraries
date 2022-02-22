using System;
using System.Threading.Tasks;

namespace OrchardCore.DisplayManagement.Views
{
    /// <summary>
    /// Helper class for generating placement strings.
    /// </summary>
    public static class ShapeResultExtensions
    {
        /// <summary>
        /// Uses <see cref="ShapeResult.Location(string)"/> to set the <paramref name="name"/> of the tab and its
        /// <paramref name="priority"/> in the order of the tabs.
        /// </summary>
        public static ShapeResult UseTab(this ShapeResult shapeResult, string name, int priority, string placement = "Parts") =>
            shapeResult.Location($"{placement}#{name}: {priority.ToTechnicalString()}");

        /// <summary>
        /// The shape will only be rendered if <paramref name="condition"/> is <see langword="true"/>. This is a
        /// shortcut for <see cref="ShapeResult.RenderWhen"/> in case the condition is known before the shape is
        /// initialized.
        /// </summary>
        public static ShapeResult RenderWhen(this ShapeResult shapeResult, bool condition) =>
            shapeResult.RenderWhen(() => Task.FromResult(condition));
    }
}
