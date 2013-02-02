using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piedone.HelpfulLibraries.Libraries.Utilities
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
