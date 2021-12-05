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

namespace Lombiq.HelpfulLibraries.Libraries.Mvc
{
    public class RouteModel
    {
        private readonly List<KeyValuePair<string, string>> _arguments;

        private readonly Lazy<bool> _isAdminLazy;
        private readonly Lazy<string> _routeLazy;

        public RouteModel(
            Type controller,
            MethodInfo action,
            IEnumerable<KeyValuePair<string, string>> arguments,
            ITypeFeatureProvider typeFeatureProvider = null)
        {
            string area;
            _arguments = arguments is List<KeyValuePair<string, string>> list ? list : arguments.ToList();
            var areaPair = _arguments.FirstOrDefault(pair => pair.Key.EqualsOrdinalIgnoreCase("area"));
            if (areaPair.Value is { } areaArgumentValue)
            {
                area = areaArgumentValue;
                _arguments.Remove(areaPair);
            }
            else
            {
                area = typeFeatureProvider?.GetFeatureForDependency(controller).Extension.Id ??
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
                action.GetCustomAttribute<RouteAttribute>()?.Name ??
                $"{area}/{controller.ControllerName()}/{action.Name}");
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

        public static RouteModel CreateFromExpression<TController>(
            Expression<Action<TController>> actionExpression,
            IEnumerable<(string Key, object Value)> additionalArguments,
            ITypeFeatureProvider typeFeatureProvider = null) =>
            CreateFromExpression(
                actionExpression,
                additionalArguments
                    .Select((key, value) => new KeyValuePair<string, string>(key, value.ToString())),
                typeFeatureProvider);

        public static RouteModel CreateFromExpression<TController>(
            Expression<Action<TController>> actionExpression,
            IEnumerable<KeyValuePair<string, string>> additionalArguments,
            ITypeFeatureProvider typeFeatureProvider = null)
        {
            var operation = (MethodCallExpression)actionExpression.Body;
            var methodParameters = operation.Method.GetParameters();

            var arguments = operation
                .Arguments
                .Select((argument, index) => new KeyValuePair<string, string>(
                    methodParameters[index].Name,
                    Expression.Lambda(argument).Compile().DynamicInvoke()?.ToString() ?? string.Empty))
                .Concat(additionalArguments);

            return new RouteModel(
                typeof(TController),
                operation.Method,
                arguments,
                typeFeatureProvider);
        }
    }
}
