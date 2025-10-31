using System;

namespace Piedone.HelpfulLibraries.Utilities
{
    public static class UriExtensions
    {
        /// <summary>
        /// Returns a protocol-relative URL, e.g. makes //orchardproject.net from http://orchardproject.net.
        /// </summary>
        public static string ToStringWithoutScheme(this Uri uri)
        {
            if (!uri.IsAbsoluteUri) return uri.ToString();

            // Include the port if it's a non-default one.
            var portString = uri.Scheme == "http" && uri.Port == 80 || uri.Scheme == "https" && uri.Port == 443
                ? string.Empty
                : ":" + uri.Port.ToString();
            return "//" + uri.Host + portString + uri.PathAndQuery;
        }
    }
}
