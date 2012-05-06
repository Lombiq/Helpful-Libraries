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
    internal class InnermostWhereFinder : ExpressionVisitor
    {
        private MethodCallExpression innermostWhereExpression;

        public MethodCallExpression GetInnermostWhere(Expression expression)
        {
            Visit(expression);
            return innermostWhereExpression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == "Where")
                innermostWhereExpression = expression;

            Visit(expression.Arguments[0]);

            return expression;
        }
    }

    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    internal class ContentTypeFinder : ExpressionVisitor
    {
        private Expression _expression;
        private List<string> _contentTypes;

        public ContentTypeFinder(Expression exppression)
        {
            _expression = exppression;
        }

        public List<string> ContentTypes
        {
            get
            {
                if (_contentTypes == null)
                {
                    _contentTypes = new List<string>();
                    Visit(_expression);
                }
                return _contentTypes;
            }
        }

        protected override Expression VisitBinary(System.Linq.Expressions.BinaryExpression be)
        {
            if (be.NodeType == ExpressionType.Equal)
            {
                if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(ContentItem), "ContentType"))
                {
                    _contentTypes.Add(ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(ContentItem), "ContentType"));
                    return be;
                }
                else
                    return base.VisitBinary(be);
            }
            else
                return base.VisitBinary(be);
        }
    }

    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    internal class ExpressionTreeModifier : ExpressionVisitor
    {
        private IQueryable<ContentItem> _queryableItems;

        internal ExpressionTreeModifier(IQueryable<ContentItem> items)
        {
            _queryableItems = items;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Type == typeof(ContentQueryable))
                return Expression.Constant(_queryableItems);
            else
                return c;
        }
    }
}
