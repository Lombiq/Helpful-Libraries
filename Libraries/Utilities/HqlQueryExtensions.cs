using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Utilities
{
    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public static class HqlQueryExtensions
    {
        /// <summary>
        /// Applies a partitioned IN() clause on the query, taken the collection of IDs. For each partition a new IN() 
        /// is created, thus the limitation on the maximal amount of arguments for IN() can be circumvented.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="ids"></param>
        /// <param name="partitionSize"></param>
        public static void WhereIdIn(this IHqlQuery query, IEnumerable<int> ids, int partitionSize = 1000)
        {
            query.Where(a => a.ContentItem(), p => p.PartitionedExpression((e, i) => e.In("Id", i), ids, partitionSize));
        }
    }
}
