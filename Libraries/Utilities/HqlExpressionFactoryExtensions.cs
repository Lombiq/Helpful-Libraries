using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Utilities
{
    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public static class HqlExpressionFactoryExtensions
    {
        /// <summary>
        /// Applies the expression in partitions of given size, ORed together.
        /// This helps to overcome limitations with certain clauses (like IN()) that only accept a specific amount of 
        /// arguments.
        /// </summary>
        /// <typeparam name="T">Type of the values used in the expression</typeparam>
        /// <param name="expressionFactory">The HQL expression factory</param>
        /// <param name="partitionExpression">Expression to apply on a partition of values</param>
        /// <param name="values">All the values to use</param>
        /// <param name="partitionSize">Determines how many values will be used for one partition</param>
        public static void PartitionedExpression<T>(this IHqlExpressionFactory expressionFactory, Action<IHqlExpressionFactory, T[]> partitionExpression, IEnumerable<T> values, int partitionSize = 1000)
        {
            BuildPartitionExpression(expressionFactory, partitionExpression, values, partitionSize);
        }

        /// <summary>
        /// Given an expression and a list of values, this method will generate an aggregated expression
        /// that ORs together the applications of the expression on each value.
        /// </summary>
        /// <param name="expressionFactory">The HQL expression factory</param>
        /// <param name="expression">The expression to apply on each value</param>
        /// <param name="values">The values to use</param>
        /// <param name="property">The path of the property that the expression will compare the value to</param>
        public static void AggregateOr(
            this IHqlExpressionFactory expressionFactory,
            Action<IHqlExpressionFactory, object, string> expression,
            object[] values,
            string property)
        {
            if (!values?.Any() ?? true) return;

            if (!values.Skip(1).Any())
            {
                expression(expressionFactory, values.First(), property);

                return;
            }

            expressionFactory.Or(
                left => expression(expressionFactory, values.First(), property),
                right => AggregateOr(right, expression, values.Skip(1).ToArray(), property));
        }


        private static void BuildPartitionExpression<T>(IHqlExpressionFactory expressionFactory, Action<IHqlExpressionFactory, T[]> partitionExpression, IEnumerable<T> values, int partitionSize)
        {
            if (values.Count() <= partitionSize)
            {
                partitionExpression(expressionFactory, values.ToArray());
                return;
            }

            expressionFactory.Or(lhs => partitionExpression(lhs, values.Take(partitionSize).ToArray()), rhs => BuildPartitionExpression(rhs, partitionExpression, values.Skip(partitionSize), partitionSize));
        }
    }
}
