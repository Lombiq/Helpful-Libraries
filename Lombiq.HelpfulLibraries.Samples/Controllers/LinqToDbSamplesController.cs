using LinqToDB;
using Lombiq.HelpfulLibraries.LinqToDb;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql;

namespace Lombiq.HelpfulLibraries.Samples.Controllers
{
    public class LinqToDbSamplesController : Controller
    {
        private readonly ISession _session;

        public LinqToDbSamplesController(ISession session) => _session = session;

        // Open this from under /Lombiq.HelpfulLibraries.Samples/LinqToDbSamples/SimpleQuery
        public async Task<ActionResult> SimpleQuery()
        {
            var result = await _session.LinqQueryAsync(
                accessor => accessor
                    .GetTable<AutoroutePartIndex>()
                    .Where(index => index.Path.Contains("a", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(index => index.Path)
                    .ToListAsync());

            return Ok(result);
        }

        // Open this from under /Lombiq.HelpfulLibraries.Samples/LinqToDbSamples/JoinQuery
        public async Task<ActionResult> JoinQuery()
        {
            // This will fetch all items under the "blog/" path. If you used the Blog recipe then these will be all blog
            // posts.
            var result = await _session.LinqQueryAsync(
                accessor =>
                    (from contentItemIndex in accessor.GetTable<ContentItemIndex>()
                     join autoroutePartIndex in accessor.GetTable<AutoroutePartIndex>()
                     on contentItemIndex.ContentItemId equals autoroutePartIndex.ContentItemId
                     where autoroutePartIndex.Path.StartsWith("blog/", StringComparison.OrdinalIgnoreCase)
                     select contentItemIndex.DisplayText)
                    .ToListAsync());

            return Ok(result);
        }
    }
}
