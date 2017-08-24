namespace Orchard.DisplayManagement.Implementation
{
    public static class ShapeDisplayingContextExtensions
    {
        /// <summary>
        /// Prevents the display of the shape currently being displayed.
        /// </summary>
        public static void HideShape(this ShapeDisplayingContext context) =>
            // This is the easiest way to hide a shape (otherwise its Placement would need to be overridden from an
            // IShapeTableEventHandler).
            context.ChildContent = new System.Web.HtmlString(string.Empty);
    }
}
