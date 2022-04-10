namespace System.Linq.Expressions;

public static class ExpressionExtensions
{
    /// <summary>
    /// Creates a new lambda expression that's identical except it doesn't have a return value.
    /// </summary>
    public static Expression<Action<TIn>> StripResult<TIn, TOut>(this Expression<Func<TIn, TOut>> expression) =>
        Expression.Lambda<Action<TIn>>(expression, expression.Parameters);
}
