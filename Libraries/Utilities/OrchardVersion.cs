using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Utilities
{
    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public static class OrchardVersion
    {
        public static Version Current()
        {
            return Assembly.GetAssembly(typeof(Orchard.Core.Common.Models.BodyPart)).GetName().Version;
        }
    }
}