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
        public static void WhereIdIn(this IHqlQuery query, IEnumerable<int> ids, int partitionSize = 1)
        {
            query.Where(a => a.ContentItem(), p => p.PartitionedExpression((e, i) => e.In("Id", i), ids, partitionSize));
        }
    }
}
