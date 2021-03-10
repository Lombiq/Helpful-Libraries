namespace OrchardCore.DisplayManagement.Views
{
    /// <summary>
    /// Helper class for generating placement strings.
    /// </summary>
    public static class ShapeResultExtensions
    {
        public static ShapeResult UseTab(this ShapeResult shapeResult, string name, int priority) =>
            shapeResult.Location($"Parts#{name}: {priority}");
    }
}
