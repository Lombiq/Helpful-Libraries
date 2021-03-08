namespace Lombiq.HelpfulLibraries.Libraries.ResourceManagement
{
    /// <summary>
    /// A service that can add static resources to the resource management pipeline.
    /// </summary>
    public interface IResourceFilterProvider
    {
        /// <summary>
        /// Adds static resources to the pipeline that will be loaded based on various criteria.
        /// </summary>
        void AddResourceFilter(ResourceFilterBuilder builder);
    }
}
