using System.Collections.Generic;
using System.Linq;

namespace Lombiq.HelpfulLibraries.AspNetCore.Security;

public static class CspHelper
{
    public static void MergeValues(IDictionary<string, string> policies, string key, params string[] sources) =>
        MergeValues(policies, key, (IEnumerable<string>)sources);

    public static void MergeValues(IDictionary<string, string> policies, string key, IEnumerable<string> sources)
    {
        var directiveValue = policies.GetMaybe(key) ?? policies.GetMaybe(ContentSecurityPolicyDirectives.DefaultSrc) ?? string.Empty;

        policies[key] = string.Join(' ', directiveValue
            .Split(' ')
            .Union(sources)
            .Distinct());
    }
}
