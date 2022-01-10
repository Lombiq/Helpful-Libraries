using Orchard;
using System;
using System.Reflection;

namespace Piedone.HelpfulLibraries.Utilities
{
    public static class OrchardVersion
    {
        public static Version Current()
        {
            return Assembly.GetAssembly(typeof(IDependency)).GetName().Version;
        }
    }
}
