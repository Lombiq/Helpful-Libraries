using OrchardCore.ContentManagement;

namespace Lombiq.HelpfulLibraries.Enums
{
    /// <summary>
    /// The visibility of the <see cref="ContentItem"/> in content management.
    /// </summary>
    public enum PublicationStatus
    {
        /// <summary>
        /// The content is visible to the end-user, eg. after pushing the Publish button.
        /// </summary>
        Published,

        /// <summary>
        /// The content is listed but not yet published, eg. after pushing the To Draft button.
        /// </summary>
        Draft,

        /// <summary>
        /// The content is deleted but remains in database as version history, eg. after pushing the To Draft button.
        /// </summary>
        Archived,

        /// <summary>
        /// The publication status is unknown, eg when the <see cref="ContentItem"/> doesn't exist.
        /// </summary>
        Unknown,
    }
}
