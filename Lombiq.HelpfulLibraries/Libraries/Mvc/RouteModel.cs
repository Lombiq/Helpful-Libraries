using Microsoft.AspNetCore.Mvc;
using OrchardCore.Admin;
using OrchardCore.Environment.Extensions;
using OrchardCore.Modules.Manifest;
using OrchardCore.Mvc.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using EnumerableExtensions = System.Collections.Generic.EnumerableExtensions;

namespace Lombiq.HelpfulLibraries.Libraries.Mvc
{
    public class RouteModel
    {
        private readonly Type _controller;
        private readonly MethodInfo _action;

        private readonly string _controllerName;
        private readonly string _actionName;

        private readonly List<KeyValuePair<string, string>> _arguments;
        private readonly string _area;

        private readonly Lazy<bool> _isAdminLazy;
        private readonly Lazy<string> _routeLazy;

        public RouteModel(
            Type controller,
            MethodInfo action,
            IEnumerable<KeyValuePair<string, string>> arguments,
            ITypeFeatureProvider typeFeatureProvider = null)
        {
            _controller = controller;
            _action = action;

            _controllerName = _controller.ControllerName();
            _actionName = _action.Name;

            _arguments = arguments is List<KeyValuePair<string, string>> list ? list : arguments.ToList();
            var areaPair = _arguments.FirstOrDefault(pair => pair.Key.EqualsOrdinalIgnoreCase("area"));
            if (areaPair.Value is { } area)
            {
                _area = area;
                _arguments.Remove(areaPair);
            }
            else
            {
                _area = typeFeatureProvider?.GetFeatureForDependency(_controller).Extension.Id ??
                        _controller.Assembly.GetCustomAttribute<ModuleNameAttribute>()?.Name ??
                        throw new InvalidOperationException(
                            "No area argument was provided and couldn't figure out the module technical name. Are " +
                            "you sure this controller belongs to an Orchard Core module?");
            }

            _isAdminLazy = new Lazy<bool>(() =>
                controller.GetCustomAttribute<AdminAttribute>() != null ||
                action.GetCustomAttribute<AdminAttribute>() != null);
            _routeLazy = new Lazy<string>(() =>
                _action.GetCustomAttribute<RouteAttribute>()?.Name ??
                $"{_area}/{_controllerName}/{_actionName}");
        }

        public override string ToString()
        {
            var prefix = _isAdminLazy.Value ? "/Admin/" : "/";
            var route = _routeLazy.Value;
            var arguments = _arguments.Any()
                ? "?" + string.Join("&", EnumerableExtensions.Select(_arguments, (key, value) => $"{key}={WebUtility.UrlEncode(value)}"))
                : string.Empty;

            return prefix + route + arguments;
        }

        public string ToString(string tenantName) => $"/{tenantName}{this}";

        public static RouteModel Create<TController, TActionResult>(
            Expression<Func<TController, TActionResult>> action,
            IEnumerable<KeyValuePair<string, string>> additionalArguments,
            ITypeFeatureProvider typeFeatureProvider = null)
        {
            var operation = (MethodCallExpression)action.Body;
            var methodParameters = operation.Method.GetParameters();

            var arguments = operation
                .Arguments
                .Select((argument, index) => new KeyValuePair<string, string>(
                    methodParameters[index].Name,
                    ((ConstantExpression)argument).Value?.ToString() ?? string.Empty))
                .Concat(additionalArguments);

            return new RouteModel(
                typeof(TController),
                operation.Method,
                arguments,
                typeFeatureProvider);
        }
    }
}
