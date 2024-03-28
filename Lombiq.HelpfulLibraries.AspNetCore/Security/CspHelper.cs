using System;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.HelpfulLibraries.AspNetCore.Security;

internal static class CspHelper
{
    public static void MergeValues(IDictionary<string, string> policies, string key, IEnumerable<Uri> sources)
    {
        var directiveValue = policies.GetMaybe(key) ?? policies.GetMaybe(ContentSecurityPolicyDirectives.DefaultSrc) ?? string.Empty;

        policies[key] = string.Join(' ', directiveValue
            .Split(' ')
            .Union(sources.Select(uri => uri.Host))
            .Distinct());
    }
}
