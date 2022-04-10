using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Microsoft.AspNetCore.Mvc.Filters;

public static class ResultExecutingContextExtensions
{
    /// <summary>
    /// Indicates if the current result is a full view rendering result (i.e. where you can properly inject shapes into
    /// the Layout).
    /// </summary>
    /// <remarks>
    /// <para>The URL /Admin/Media/MediaApplication from OrchardCore.Media will be a full view rendering though.</para>
    /// </remarks>
    public static bool IsNotFullViewRendering(this ResultExecutingContext context) =>
        context.Result is not ViewResult and not PageResult;
}
