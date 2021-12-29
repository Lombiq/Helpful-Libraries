using System;

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
        public static ShapeResult UseTab(this ShapeResult shapeResult, string name, int priority) =>
            shapeResult.Location($"Parts#{name}: {priority.ToTechnicalString()}");
    }
}
