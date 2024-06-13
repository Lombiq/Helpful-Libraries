using System.Collections.Generic;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives.CommonValues;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Enable the <see cref="UnsafeEval"/> value for the <see cref="ScriptSrc"/> directive. This is necessary to evaluate
/// dynamic (not precompiled) templates. These are extensively used in stock Orchard Core. Also in many third party
/// modules where the DOM HTML template may contain Razor generated content.
/// </summary>
public class VueContentSecurityPolicyProvider : ResourceManagerContentSecurityPolicyProvider
{
    protected override string ResourceType => "script";
    protected override string ResourceName => "vuejs";
    protected override IReadOnlyCollection<string> DirectiveNameChain { get; } = [ScriptSrc];
    protected override string DirectiveValue => UnsafeEval;
}
