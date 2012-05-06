using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Contents.Linq
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    internal class ContentQueryContext
    {
        // Executes the expression tree that is passed to it.
        internal static object Execute(Expression expression, bool isEnumerable, IContentQuery<ContentItem> query)
        {
            // The expression must represent a query over the data source.
            if (!IsQueryOverDataSource(expression))
                throw new InvalidQueryException("No query over the data source was specified.");

            var whereExpression = new InnermostWhereFinder().GetInnermostWhere(expression);
            if (whereExpression != null) ProcessWhere(whereExpression, query);

            if (isEnumerable)
            {
                // The call was to ToList() ToArray() etc. that awaits a collections. That means, the query should be evaluated.
                return query.List();
            }
            else if (IsScalar(expression))
            {
                return ProcessScalar(expression, query);
            }

            // The below should happen id we hit something that's not supported, except that all not evaluated expressions should be copied,
            // not just the innermost.
            var queryableContents = query.List().AsQueryable<ContentItem>();

            // Copy the expression tree that was passed in, changing only the first
            // argument of the innermost MethodCallExpression.
            var treeCopier = new ExpressionTreeModifier(queryableContents);
            var newExpressionTree = treeCopier.Visit(expression);

            if (isEnumerable)
                return queryableContents.Provider.CreateQuery(newExpressionTree);
            else
                return queryableContents.Provider.Execute(newExpressionTree);
        }

        private static bool IsQueryOverDataSource(Expression expression)
        {
            // If expression represents an unqueried IQueryable data source instance,
            // expression is of type ConstantExpression, not MethodCallExpression.
            return (expression is MethodCallExpression);
        }

        private static void ProcessWhere(MethodCallExpression whereExpression, IContentQuery<ContentItem> query)
        {
            var lambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;
            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            var contentTypes = new ContentTypeFinder(lambdaExpression.Body).ContentTypes;
            if (contentTypes.Count != 0) query = query.ForType(contentTypes.ToArray());
        }
        
        /// <summary>
        /// Checks if the query should return a scalar value, like with Count(), Max()...
        /// </summary>
        private static bool IsScalar(Expression expression)
        {
            if (!(expression is MethodCallExpression)) return false;

            var scalars = new string[] { "Count", "Max", "Min", "Sum" };
            return scalars.Contains(((MethodCallExpression)expression).Method.Name);
        }

        private static object ProcessScalar(Expression expression, IContentQuery<ContentItem> query)
        {
            var methodExpression = (MethodCallExpression)expression;

            switch (methodExpression.Method.Name)
            {
                case "Count":
                    return query.Count();
                default:
                    throw new NotImplementedException("The method " + methodExpression.Method.Name + " is not supported.");
            }
        }
    }
}
