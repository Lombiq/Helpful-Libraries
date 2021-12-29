using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Orchard;

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