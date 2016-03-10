using System;
using System.Linq;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Utilities
{
    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public static class UriHelper
    {
        /// <summary>
        /// Combines uri segments with forward slashes (much like Path.Combine() for local paths).
        /// </summary>
        /// <param name="segments">The segments to combine.</param>
        public static string Combine(params string[] segments)
        {
            var isProtocolRelative = segments.First().StartsWith("//");
            var joined = string.Join("/", segments.Select(f => f.Trim().Trim('/')));
            if (isProtocolRelative) joined = "//" + joined;
            if (!string.IsNullOrEmpty(segments.Last()) && segments.Last().Last() == '/' && joined.Last() != '/') return joined + "/";
            return joined;
        }

        /// <summary>
        /// Creates a Uri object from a string that can contain an absolute (even in protocol-relative form) or relative
        /// URL.
        /// </summary>
        /// <param name="uriString">The string to create the Uri from.</param>
        /// <returns></returns>
        public static Uri CreateUri(string uriString)
        {
            if (string.IsNullOrEmpty(uriString)) throw new ArgumentNullException("uriString");

            if (uriString.StartsWith("//")) uriString = "http:" + uriString;
            if (Uri.IsWellFormedUriString(uriString, UriKind.Absolute)) return new Uri(uriString, UriKind.Absolute);
            return new Uri(uriString, UriKind.Relative);
        }
    }
}
