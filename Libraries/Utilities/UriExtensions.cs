using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Libraries.Utilities
{
    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public static class UriExtensions
    {
        public static string ToStringWithoutScheme(this Uri uri)
        {
            if (!uri.IsAbsoluteUri) return uri.ToString();
            return "//" + uri.Host + uri.PathAndQuery;
        }
    }
}
