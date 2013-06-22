using Orchard;

namespace Piedone.HelpfulLibraries.Utilities
{
    /// <summary>
    /// Service for managing ORM mappings
    /// </summary>
    public interface IMappingsManager : IDependency
    {
        /// <summary>
        /// Clears cached mappings and thus forces the regeneration of the cache
        /// </summary>
        void Clear();
    }
}
