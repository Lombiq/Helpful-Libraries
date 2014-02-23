using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;
using Orchard.Localization;

namespace Piedone.HelpfulLibraries.Libraries.Utilities
{
    /// <summary>
    /// A simple container for the Localizer delegate. Resolve this interface from the DI container if you want to resolve a Localizer
    /// in-place instead of through property injection.
    /// </summary>
    public interface ILocalizerDependency : IDependency
    {
        Localizer T { get; }
    }


    public class LocalizerDependency : ILocalizerDependency
    {
        public Localizer T { get; set; }


        public LocalizerDependency()
        {
            T = NullLocalizer.Instance;
        }
    }
}
