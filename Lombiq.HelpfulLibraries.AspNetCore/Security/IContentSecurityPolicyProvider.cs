using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;

namespace Lombiq.HelpfulLibraries.AspNetCore.Security;

/// <summary>
/// A service for updating the dictionary that will be turned into the <c>Content-Security-Policy</c> header value by
/// <see cref="ApplicationBuilderExtensions.UseContentSecurityPolicyHeader"/>.
/// </summary>
public interface IContentSecurityPolicyProvider
{
    /// <summary>
    /// Updates the <paramref name="securityPolicies"/> dictionary where the keys are the <c>Content-Security-Policy</c>
    /// directives names and the values are the matching directive values.
    /// </summary>
    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context) =>
        ValueTask.CompletedTask;

    /// <summary>
    /// Returns a value indicating whether the <c>Content-Security-Policy</c> header should not be added to the current
    /// page.
    /// </summary>
    public ValueTask<bool> ShouldSuppressHeaderAsync(HttpContext context) =>
        new(result: false);

    /// <summary>
    /// Returns the first non-empty directive from the <paramref name="names"/> or <see cref="DefaultSrc"/> or an empty
    /// string.
    /// </summary>
    [Obsolete($"Use the method in the {nameof(ContentSecurityPolicyProvider)} static class instead.")]
    public static string GetDirective(IDictionary<string, string> securityPolicies, params string[] names) =>
        ContentSecurityPolicyProvider.GetDirective(securityPolicies, names.AsEnumerable());
}

public static class ContentSecurityPolicyProvider
{
    /// <summary>
    /// Returns the first non-empty directive from the <paramref name="names"/> or <see cref="DefaultSrc"/> or an empty
    /// string.
    /// </summary>
    public static string GetDirective(IDictionary<string, string> securityPolicies, params string[] names) =>
        GetDirective(securityPolicies, names.AsEnumerable());

    /// <inheritdoc cref="GetDirective(IDictionary{string,string},string[])"/>
    public static string GetDirective(IDictionary<string, string> securityPolicies, IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            if (securityPolicies.TryGetValue(name, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return securityPolicies.GetMaybe(DefaultSrc) ?? string.Empty;
    }

    /// <summary>
    /// Updates the directive (the first entry of the <paramref name="directiveNameChain"/>) by merging its space
    /// separated values with the values from <paramref name="otherValues"/>.
    /// </summary>
    public static void MergeDirectiveValues(
        IDictionary<string, string> securityPolicies,
        IEnumerable<string> directiveNameChain,
        params string[] otherValues)
    {
        var nameChain = directiveNameChain.AsList();
        securityPolicies[nameChain[0]] = GetDirective(securityPolicies, nameChain).MergeWordSets(otherValues);
    }
}
