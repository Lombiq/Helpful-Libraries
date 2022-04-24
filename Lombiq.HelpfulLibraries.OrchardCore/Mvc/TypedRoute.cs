using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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

namespace Lombiq.HelpfulLibraries.OrchardCore.Mvc;

public class TypedRoute
{
    private static readonly ConcurrentDictionary<string, TypedRoute> _cache = new();

    private readonly string _area;
    private readonly Type _controller;
    private readonly MethodInfo _action;
    private readonly List<KeyValuePair<string, string>> _arguments;

    private readonly Lazy<bool> _isAdminLazy;
    private readonly Lazy<string> _routeLazy;

    public TypedRoute(
        MethodInfo action,
        IEnumerable<KeyValuePair<string, string>> arguments,
        ITypeFeatureProvider typeFeatureProvider = null)
    {
        if (action?.DeclaringType is not { } controller)
        {
            throw new InvalidOperationException(
                $"{action?.Name ?? nameof(action)}'s {nameof(action.DeclaringType)} cannot be null.");
        }

        _controller = controller;
        _action = action;

        _arguments = arguments is List<KeyValuePair<string, string>> list ? list : arguments.ToList();
        var areaPair = _arguments.FirstOrDefault(pair => pair.Key.EqualsOrdinalIgnoreCase("area"));
        if (areaPair.Value is { } areaArgumentValue)
        {
            _area = areaArgumentValue;
            _arguments.Remove(areaPair);
        }
        else
        {
            _area = typeFeatureProvider?.GetFeatureForDependency(controller).Extension.Id ??
                    controller.Assembly.GetCustomAttribute<ModuleNameAttribute>()?.Name ??
                    controller.Assembly.GetCustomAttribute<ModuleMarkerAttribute>()?.Name ??
                    throw new InvalidOperationException(
                        "No area argument was provided and couldn't figure out the module technical name. Are " +
                        "you sure this controller belongs to an Orchard Core module?");
        }

        _isAdminLazy = new Lazy<bool>(() =>
            controller.GetCustomAttribute<AdminAttribute>() != null ||
            action.GetCustomAttribute<AdminAttribute>() != null);
        _routeLazy = new Lazy<string>(() =>
            action.GetCustomAttribute<RouteAttribute>()?.Template is { } route && !string.IsNullOrWhiteSpace(route)
                ? GetRoute(route)
                : $"{_area}/{controller.ControllerName()}/{action.GetCustomAttribute<ActionNameAttribute>()?.Name ?? action.Name}");
    }

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

    public override string ToString()
    {
        var prefix = _isAdminLazy.Value ? "/Admin/" : "/";
        var route = _routeLazy.Value;
        var arguments = _arguments.Any()
            ? "?" + string.Join("&", _arguments.Select((key, value) => $"{key}={WebUtility.UrlEncode(value)}"))
            : string.Empty;

        return prefix + route + arguments;
    }

    public string ToString(string tenantName) =>
        string.IsNullOrWhiteSpace(tenantName) || tenantName.EqualsOrdinalIgnoreCase("Default")
            ? ToString()
            : $"/{tenantName}{this}";

    private string GetRoute(string route)
    {
        var argumentsCopy = _arguments.ToList(); // This way modifying _arguments in the loop won't cause problems.
        foreach (var (name, value) in argumentsCopy)
        {
            var placeholder = $"{{{name}}}";
            if (!route.ContainsOrdinalIgnoreCase(placeholder)) continue;

            route = route.ReplaceOrdinalIgnoreCase(placeholder, WebUtility.UrlEncode(value));
            _arguments.RemoveAll(pair => pair.Key == name);
        }

        return route;
    }

    public static implicit operator RouteValueDictionary(TypedRoute route) =>
        new(route._arguments)
        {
            ["area"] = route._area,
            ["controller"] = route._controller.ControllerName(),
            ["action"] = route._action.Name,
        };

    public static TypedRoute CreateFromExpression<TController>(
        Expression<Action<TController>> actionExpression,
        IEnumerable<(string Key, object Value)> additionalArguments,
        ITypeFeatureProvider typeFeatureProvider = null)
        where TController : ControllerBase =>
        CreateFromExpression(
            actionExpression,
            additionalArguments.Select((key, value) => new KeyValuePair<string, string>(key, value.ToString())),
            typeFeatureProvider);

    public static TypedRoute CreateFromExpression<TController>(
        Expression<Action<TController>> action,
        IEnumerable<KeyValuePair<string, string>> additionalArguments,
        ITypeFeatureProvider typeFeatureProvider = null)
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
            .Concat(additionalArguments)
            .ToList();

        var key = string.Join(
            separator: '|',
            typeof(TController),
            operation.Method,
            string.Join(",", arguments.Select(pair => $"{pair.Key}={pair.Value}")));

        return _cache.GetOrAdd(
            key,
            _ => new TypedRoute(
                operation.Method,
                arguments,
                typeFeatureProvider));
    }

    private static string ValueToString(object value) =>
        value switch
        {
            null => null,
            string text => text,
            DateTime date => date.ToString("s", CultureInfo.InvariantCulture),
            byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal =>
                string.Format(CultureInfo.InvariantCulture, "{0}", value),
            _ => JsonConvert.SerializeObject(value),
        };
}
