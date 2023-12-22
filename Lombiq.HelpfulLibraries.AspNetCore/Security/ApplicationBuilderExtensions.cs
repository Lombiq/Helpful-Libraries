using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder AddContentSecurityPolicyHeader(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            var securityPolicies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [DefaultSrc] = CommonValues.Self,
            };

            foreach (var provider in context.RequestServices.GetService<IEnumerable<IContentSecurityPolicyProvider>>())
            {
                await provider.UpdateAsync(securityPolicies);
            }

            var policy = string.Join("; ", securityPolicies.Select((key, value) => $"{key} {value}"));
            context.Response.Headers.Add("Content-Security-Policy", policy);

            await next();
        });
}
