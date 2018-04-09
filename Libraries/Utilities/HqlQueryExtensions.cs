using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using System.Collections.Generic;

namespace Piedone.HelpfulLibraries.Utilities
{
    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public static class HqlQueryExtensions
    {
        /// <summary>
        /// Applies a partitioned IN() clause on the query, taken the collection of IDs. For each partition a new IN() 
        /// is created, thus the limitation on the maximal amount of arguments for IN() can be circumvented.
        /// </summary>
        public static void WhereIdIn(this IHqlQuery query, IEnumerable<int> ids, int partitionSize = 1000)
        {
            query.Where(a => a.ContentItem(), p =>
                p.PartitionedExpression((e, i) => e.In("Id", i), ids, partitionSize));
        }

        /// <summary>
        /// Applies a partitioned NOT IN() clause on the query, taken the collection of IDs. For each partition a new NOT IN() 
        /// is created, thus the limitation on the maximal amount of arguments for IN() can be circumvented.
        /// </summary>
        public static void WhereIdNotIn(this IHqlQuery query, IEnumerable<int> ids, int partitionSize = 1000)
        {
            query.Where(a => a.ContentItem(), p =>
                p.PartitionedExpression((e, i) => e.Not(inner => inner.In("Id", i)), ids, partitionSize, true));
        }
    }
}
