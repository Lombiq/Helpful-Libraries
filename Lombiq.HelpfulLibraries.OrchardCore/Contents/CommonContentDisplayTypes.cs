using OrchardCore.ContentManagement.Display;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

/// <summary>
/// Values that can be used with <see cref="IContentItemDisplayManager.BuildDisplayAsync"/> or <see
/// cref="OrchardRazorHelperExtensions.DisplayAsync"/> to safely select the correct display type.
/// </summary>
public static class CommonContentDisplayTypes
{
    public const string Detail = nameof(Detail);
    public const string Summary = nameof(Summary);
    public const string SummaryAdmin = nameof(SummaryAdmin);
}
