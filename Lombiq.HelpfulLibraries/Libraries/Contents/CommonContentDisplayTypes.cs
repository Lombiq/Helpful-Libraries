using OrchardCore.ContentManagement.Display;

namespace Lombiq.HelpfulLibraries.Libraries.Contents
{
    /// <summary>
    /// Values that can be used with <see cref="IContentItemDisplayManager.BuildDisplayAsync"/> or
    /// <see cref="OrchardRazorHelperExtensions.DisplayAsync"/>.
    /// </summary>
    public static class CommonContentDisplayTypes
    {
        public const string Summary = nameof(Summary);
        public const string SummaryAdmin = nameof(SummaryAdmin);
    }
}
