using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using System.Linq.Expressions;
using System.Collections;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Contents.Linq
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public class ContentQueryable : IQueryable<ContentItem>
    {
        public Type ElementType
        {
            get { return typeof(ContentItem); }
        }

        public Expression Expression { get; private set; }
        public IQueryProvider Provider { get; private set; }

        public ContentQueryable(IContentQuery<ContentItem> query)
        {
            Provider = new ContentQueryProvider(query);
            Expression = Expression.Constant(this);
        }

        internal ContentQueryable(IQueryProvider provider, Expression expression)
        {
            Provider = provider;
            Expression = expression;
        }

        public IEnumerator<ContentItem> GetEnumerator()
        {
            return Provider.Execute<IEnumerable<ContentItem>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
