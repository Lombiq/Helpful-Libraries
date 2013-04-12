using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Libraries.Utilities
{
    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public static class UriHelper
    {
        /// <summary>
        /// Combines uri segments with forward slashes (much like Path.Combine() for local paths)
        /// </summary>
        /// <param name="segments">The segments to combine</param>
        public static string Combine(params string[] segments)
        {
            return string.Join("/", segments.Select(f => f.Trim().Trim('/')));
        }
    }
}
