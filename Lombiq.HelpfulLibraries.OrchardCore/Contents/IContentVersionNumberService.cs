using OrchardCore.ContentManagement;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

/// <summary>
/// Service for getting the version numbers of a content item or a specific version of it.
/// </summary>
public interface IContentVersionNumberService
{
    /// <summary>
    /// Returns the current largest version number for the content item.
    /// </summary>
    Task<int> GetLatestVersionNumberAsync(string contentItemId);

    /// <summary>
    /// Returns the version number of a specific version.
    /// </summary>
    Task<int> GetCurrentVersionNumberAsync(string contentItemId, string contentItemVersionId);
}

public static class ContentVersionNumberServiceExtensions
{
    /// <summary>
    /// Returns the current largest version number for an <see cref="IContent"/> object.
    /// </summary>
    public static Task<int> GetLatestVersionNumberAsync(
        this IContentVersionNumberService service,
        IContent content) =>
        service.GetLatestVersionNumberAsync(content?.ContentItem?.ContentItemId);

    /// <summary>
    /// Returns the version number of an <see cref="IContent"/> object's specific version.
    /// </summary>
    public static Task<int> GetCurrentVersionNumberAsync(
        this IContentVersionNumberService service,
        IContent content) =>
        service.GetCurrentVersionNumberAsync(
            content?.ContentItem?.ContentItemId,
            content?.ContentItem?.ContentItemVersionId);
}
