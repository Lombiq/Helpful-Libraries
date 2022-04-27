using OrchardCore.ContentManagement;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

/// <summary>
/// The status of the <see cref="ContentItem"/>.
/// </summary>
public enum PublicationStatus
{
    /// <summary>
    /// Only used for querying to return all regardless of publication status or to indicate that the status was unset.
    /// </summary>
    Any,

    /// <summary>
    /// The content published (eg. after clicking the Publish button).
    /// </summary>
    Published,

    /// <summary>
    /// The content is draft (eg. after clicking the Save Draft button).
    /// </summary>
    Draft,

    /// <summary>
    /// Only used for querying to return either published or draft items.
    /// </summary>
    Latest,

    /// <summary>
    /// The content is deleted but remains in database as version history.
    /// </summary>
    Deleted,
}
