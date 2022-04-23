using LinqToDB;
using Lombiq.HelpfulLibraries.LinqToDb;
using Lombiq.HelpfulLibraries.Samples.Models;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql;

namespace Lombiq.HelpfulLibraries.Samples.Controllers;

// Some examples of querying the database with Lombiq.HelpfulLibraries.LinqToDb.
public class LinqToDbSamplesController : Controller
{
    private readonly ISession _session;

    public LinqToDbSamplesController(ISession session) => _session = session;

    // A simple query on AutoroutePartIndex. Open this from under
    // /Lombiq.HelpfulLibraries.Samples/LinqToDbSamples/SimpleQuery
    public async Task<IActionResult> SimpleQuery()
    {
        var result = await _session.LinqQueryAsync(
            accessor => accessor
                .GetTable<AutoroutePartIndex>()
                .Where(index => index.Path.Contains('a', StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(index => index.Path)
                .ToListAsync(HttpContext.RequestAborted));

        return Ok(result);
    }

    // A more complex query with an SQL JOIN. Open this from under
    // /Lombiq.HelpfulLibraries.Samples/LinqToDbSamples/JoinQuery
    public async Task<IActionResult> JoinQuery()
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
                .ToListAsync(HttpContext.RequestAborted));

        return Ok(result);
    }

    // CRUD operations. Or rather, it's CUD but you've seen enough read operations above. Open this from under
    // /Lombiq.HelpfulLibraries.Samples/LinqToDbSamples/Crud
    public async Task<IActionResult> Crud()
    {
        var insertedCount = await _session.LinqTableQueryAsync<BookRecord, int>(table => table
            .InsertAsync(
                () => new BookRecord
                {
                    Title = "Twenty Thousand Leagues Under the Seas",
                    Author = "Jules Verne",
                },
                HttpContext.RequestAborted));

        var modifiedCount = await _session.LinqTableQueryAsync<BookRecord, int>(table => table
            .Where(record => record.Title == "Twenty Thousand Leagues Under the Seas")
            .Set(record => record.Title, "Around the World in Eighty Days")
            .UpdateAsync(HttpContext.RequestAborted));

        var deletedCount = await _session.LinqTableQueryAsync<BookRecord, int>(table => table
            .Where(record => record.Author == "Jules Verne")
            .DeleteAsync(HttpContext.RequestAborted));

        return Ok(FormattableString.Invariant(
            $"Inserted: {insertedCount}, modified: {modifiedCount}, deleted: {deletedCount}."));
    }
}
