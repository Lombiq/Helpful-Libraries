using OrchardCore.ContentManagement;

namespace Lombiq.HelpfulLibraries.Libraries.Contents
{
    /// <summary>
    /// The status of the <see cref="ContentItem"/>.
    /// </summary>
    public enum PublicationStatus
    {
        /// <summary>
        /// The content published (eg. after clicking the Publish button).
        /// </summary>
        Published,

        /// <summary>
        /// The content is draft (eg. after clicking the Save Draft button).
        /// </summary>
        Draft,

        /// <summary>
        /// The content is deleted but remains in database as version history.
        /// </summary>
        Deleted,
    }
}
