using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.Environment.Extensions;
using OrchardCore.Modules.Manifest;
using OrchardCore.Mvc.Core.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace Lombiq.HelpfulLibraries.OrchardCore.Mvc;

public class TypedRoute
{
    private static readonly ConcurrentDictionary<string, TypedRoute> _cache = new();

    private readonly string _area;
    private readonly Type _controller;
    private readonly MethodInfo _action;
    private readonly IReadOnlyList<KeyValuePair<string, string>> _arguments;

    private readonly string _prefix = "/";

    private TypedRoute(
        Type controller,
        MethodInfo action,
        List<KeyValuePair<string, string>> arguments,
        IServiceProvider serviceProvider = null)
    {
        if (arguments.Find(pair => pair.Key.EqualsOrdinalIgnoreCase("area")) is { Value: { } value } area)
        {
            _area = value;

            // It is safe to edit arguments here but treat it read-only everywhere else, because it's always locally
            // created in CreateFromExpression(), which is the only caller of this this private constructor.
            arguments.Remove(area);
        }
        else
        {
            var typeFeatureProvider = serviceProvider?.GetService<ITypeFeatureProvider>();

            // The fallbacks are only in case either the service provider or the ITypeFeatureProvider are missing. If
            // the service is available but can't resolve the feature for the provided controller it will throw. This is
            // good, because in that case the resulting link would not work anyway.
            _area = typeFeatureProvider?.GetFeatureForDependency(controller).Extension.Id ??
                    controller.Assembly.GetCustomAttribute<ModuleNameAttribute>()?.Name ??
                    controller.Assembly.GetCustomAttribute<ModuleMarkerAttribute>()?.Name ??
                    throw new InvalidOperationException(
                        $"No \"area\" argument was provided and couldn't figure out the module technical name. Are " +
                        $"you sure the \"{controller.Name}\" controller belongs to an Orchard Core module?");
        }

        var isAdmin = controller.GetCustomAttribute<AdminAttribute>() != null || action.GetCustomAttribute<AdminAttribute>() != null;
        if (isAdmin && action.GetCustomAttribute(typeof(RouteAttribute)) == null)
        {
            _prefix = $"/{(serviceProvider?.GetService<IOptions<AdminOptions>>()?.Value ?? new AdminOptions())!.AdminUrlPrefix}/";
        }

        _controller = controller;
        _action = action;
        _arguments = arguments;
    }

    /// <summary>
    /// Creates a relative URL based on the given <paramref name="httpContext"/>, the area, and the names of the
    /// <c>action</c> and the <c>controller</c>.
    /// </summary>
    public string ToString(HttpContext httpContext)
    {
        var linkGenerator = httpContext.RequestServices.GetRequiredService<LinkGenerator>();
        var arguments = new RouteValueDictionary(_arguments) { ["area"] = _area };

        return linkGenerator.GetUriByAction(
            httpContext,
            _action.Name,
            _controller.ControllerName(),
            arguments);
    }

    /// <summary>
    /// Creates a local URL using a prefix, the current route, and other arguments.
    /// </summary>
    public override string ToString()
    {
        var routeTemplate = _action.GetCustomAttribute<RouteAttribute>()?.Template ??
            _action.GetCustomAttribute<AdminAttribute>()?.Template;
        var (route, arguments) = routeTemplate != null && !string.IsNullOrWhiteSpace(routeTemplate)
            ? GetRouteFromTemplate(routeTemplate, _arguments)
            : ($"{_area}/{_controller.ControllerName()}/{_action.GetCustomAttribute<ActionNameAttribute>()?.Name ?? _action.Name}", _arguments);

        var queryString = arguments.Any()
            ? "?" + string.Join('&', arguments.Select((key, value) => $"{key}={WebUtility.UrlEncode(value)}"))
            : string.Empty;

        return _prefix + route + queryString;
    }

    /// <summary>
    /// Creates a local URL on a tenant using the provided <paramref name="tenantName"/>. If
    /// <paramref name="tenantName"/> is empty or "<c>Default</c>", creates a local URL using a prefix, the current
    /// route, and other arguments.
    /// </summary>
    public string ToString(string tenantName) =>
        string.IsNullOrWhiteSpace(tenantName) || tenantName.EqualsOrdinalIgnoreCase("Default")
            ? ToString()
            : $"/{tenantName}{this}";

    /// <summary>
    /// Resolves a route template such as <c>[Route("DataTable/{providerName}/{queryId?}")]</c>.
    /// </summary>
    /// <returns>
    /// The final route with the template strings substituted from <paramref name="arguments"/>, and the list of pairs
    /// not used up by this substitution. The latter can be added to the query string of the final URL.
    /// </returns>
    private static (string Route, IReadOnlyList<KeyValuePair<string, string>> OtherArguments) GetRouteFromTemplate(
        string routeTemplate,
        IReadOnlyList<KeyValuePair<string, string>> arguments)
    {
        if (!routeTemplate.Contains('{')) return (routeTemplate, arguments);

        var otherArguments = new List<KeyValuePair<string, string>>();

        foreach (var pair in arguments)
        {
            if (routeTemplate.RegexMatch($@"{{\s*{pair.Key}\s*\??\s*}}") is { Success: true, Groups: { } match })
            {
                routeTemplate = routeTemplate.ReplaceOrdinalIgnoreCase(
                    match[0].Value,
                    WebUtility.UrlEncode(pair.Value));
            }
            else
            {
                otherArguments.Add(pair);
            }
        }

        // Remove unmatched optional argument templates.
        routeTemplate = routeTemplate.RegexReplace(@"{[^?}]+\?\s*}", string.Empty);

        return (routeTemplate, otherArguments);
    }

    public static implicit operator RouteValueDictionary(TypedRoute route) =>
        new(route._arguments)
        {
            ["area"] = route._area,
            ["controller"] = route._controller.ControllerName(),
            ["action"] = route._action.Name,
        };

    /// <summary>
    /// Creates and returns a new <see cref="TypedRoute"/> using the provided <paramref name="actionExpression"/>,
    /// also adding it to the cache.
    /// </summary>
    /// <param name="actionExpression">The action expression whose arguments are used for the process.</param>
    /// <param name="additionalArguments">Additional arguments to add to the route and the key in the cache.</param>
    public static TypedRoute CreateFromExpression<TController>(
        Expression<Action<TController>> actionExpression,
        IEnumerable<(string Key, object Value)> additionalArguments,
        IServiceProvider serviceProvider = null)
        where TController : ControllerBase =>
        CreateFromExpression(
            actionExpression,
            additionalArguments.Select((key, value) => new KeyValuePair<string, string>(key, value.ToString())),
            serviceProvider);

    /// <summary>
    /// Creates and returns a new <see cref="TypedRoute"/> using the provided <paramref name="action"/> expression,
    /// also adding it to the cache.
    /// </summary>
    /// <param name="action">The action expression whose arguments are used for the process.</param>
    /// <param name="additionalArguments">Additional arguments to add to the route and the key in the cache.</param>
    public static TypedRoute CreateFromExpression<TController>(
        Expression<Action<TController>> action,
        IEnumerable<KeyValuePair<string, string>> additionalArguments = null,
        IServiceProvider serviceProvider = null)
        where TController : ControllerBase
    {
        Expression actionExpression = action;
        while (actionExpression is LambdaExpression { Body: not MethodCallExpression } lambdaExpression)
        {
            actionExpression = lambdaExpression.Body;
        }

        var operation = (MethodCallExpression)((LambdaExpression)actionExpression).Body;
        var methodParameters = operation.Method.GetParameters();

        var arguments = operation
            .Arguments
            .Select((argument, index) => new KeyValuePair<string, string>(
                methodParameters[index].Name,
                ValueToString(Expression.Lambda(argument).Compile().DynamicInvoke())))
            .Where(pair => pair.Value != null)
            .Concat(additionalArguments ?? Enumerable.Empty<KeyValuePair<string, string>>())
            .ToList();

        var key = string.Join(
            separator: '|',
            typeof(TController).FullName,
            operation.Method,
            string.Join(',', arguments.Select(pair => $"{pair.Key}={pair.Value}")));

        if (serviceProvider?.GetService<IMemoryCache>() is { } cache)
        {
            return cache.GetOrCreate(
                key,
                _ => new TypedRoute(
                    typeof(TController),
                    operation.Method,
                    arguments,
                    serviceProvider));
        }

        return _cache.GetOrAdd(
            key,
            _ => new TypedRoute(
                typeof(TController),
                operation.Method,
                arguments,
                serviceProvider));
    }

    private static string ValueToString(object value) =>
        value switch
        {
            null => null,
            string text => text,
            DateTime date => date.ToString("s", CultureInfo.InvariantCulture),
            byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal =>
                string.Format(CultureInfo.InvariantCulture, "{0}", value),
            _ => JsonSerializer.Serialize(value),
        };
}
