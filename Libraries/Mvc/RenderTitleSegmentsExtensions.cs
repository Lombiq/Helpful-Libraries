using Microsoft.AspNetCore.Html;

namespace OrchardCore.DisplayManagement.Razor
{
    public static class RenderTitleSegmentsExtensions
    {
        /// <summary>
        /// Renders title segments using dynamic type.
        /// </summary>
        /// <param name="segment">Text of the title segment.</param>
        /// <param name="position">Position of the title segment.</param>
        /// <param name="separator">Separator between segments.</param>
        /// <returns></returns>
        public static IHtmlContent RenderDynamicTitleSegments(
            this RazorPage<dynamic> razorPage,
            dynamic segment,
            string position = "0",
            IHtmlContent separator = null) => razorPage.RenderTitleSegments(segment.ToString(), position, separator);
    }
}
