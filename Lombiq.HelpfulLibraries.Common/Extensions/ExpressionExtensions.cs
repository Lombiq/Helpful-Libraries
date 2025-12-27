using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace System.Linq.Expressions;

public static class ExpressionExtensions
{
    /// <summary>
    /// Creates a new lambda expression that's identical except it doesn't have a return value.
    /// </summary>
    public static Expression<Action<TIn>> StripResult<TIn, TOut>(this Expression<Func<TIn, TOut>> expression) =>
        Expression.Lambda<Action<TIn>>(expression.Body, expression.Parameters);

    /// <summary>
    /// Gets information about a <see cref="Expression{TDelegate}"/> which should contain just one method call. The <see
    /// cref="MethodInfo"/> and argument collection is extracted and returned.
    /// </summary>
    public static (MethodInfo Method, List<KeyValuePair<string, string?>> Arguments) GetMethodCallInfo(this Expression expression)
    {
        static string? ValueToString(object? value) =>
            value switch
            {
                null => null,
                string text => text,
                DateTime date => date.ToString("s", CultureInfo.InvariantCulture),
                byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal =>
                    string.Format(CultureInfo.InvariantCulture, "{0}", value),
                _ => JsonSerializer.Serialize(value),
            };

        while (expression is LambdaExpression { Body: not MethodCallExpression } lambdaExpression)
        {
            expression = lambdaExpression.Body;
        }

        var operation = (MethodCallExpression)((LambdaExpression)expression).Body;
        var methodParameters = operation.Method.GetParameters();

        var arguments = operation
            .Arguments
            .Select((argument, index) => new
            {
                methodParameters[index].Name,
                Value = ValueToString(Expression.Lambda(argument).Compile().DynamicInvoke()),
            })
            .Where(pair => !string.IsNullOrEmpty(pair.Name) && !string.IsNullOrEmpty(pair.Value))
            .Select(pair => new KeyValuePair<string, string?>(pair.Name!, pair.Value!))
            .ToList();

        return (operation.Method, arguments);
    }
}
