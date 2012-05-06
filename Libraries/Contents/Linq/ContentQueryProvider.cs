using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using System.Linq.Expressions;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Contents.Linq
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public class ContentQueryProvider : IQueryProvider
    {
        private readonly IContentQuery<ContentItem> _query;

        public ContentQueryProvider(IContentQuery<ContentItem> query)
        {
            _query = query;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new ContentQueryable(this, expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return (IQueryable<TElement>)new ContentQueryable(this, expression);
        }

        public object Execute(Expression expression)
        {
            return Execute<ContentItem>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var isEnumerable = (typeof(TResult).Name == "IEnumerable`1");
            return (TResult)ContentQueryContext.Execute(expression, isEnumerable, _query);
        }
    }
}
