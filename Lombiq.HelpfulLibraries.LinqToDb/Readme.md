# LINQ to DB - Lombiq Helpful Libraries for Orchard Core



## About

With the help of this project you can write LINQ expressions and run it with an `ISession` extension method to query from the DB instead of writing plain SQL queries. Uses the [LINQ to DB project](https://linq2db.github.io/).

You can watch a demo video of the project [here](https://www.youtube.com/watch?v=ldJOdCSsWJo).


## Documentation

Use the `LinqQueryAsync` ISession extension method for running LINQ syntax-based DB queries. Check out its documentation inline.

### Sample controller

```csharp
public class LinqToDbSamplesController
{
    private readonly ISession _session;
        
    public LinqToDbSamplesController(ISession session) => _session = session;

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

    public async Task<ActionResult> JoinQuery()
    {
        var result = await _session.LinqQueryAsync(
            accessor =>
                (from contentItemIndex in accessor.GetTable<ContentItemIndex>()
                    join autoroutePartIndex in accessor.GetTable<AutoroutePartIndex>()
                        on contentItemIndex.ContentItemId equals autoroutePartIndex.ContentItemId
                    where autoroutePartIndex.Path.StartsWith("tags/", StringComparison.OrdinalIgnoreCase)
                    select contentItemIndex.DisplayText)
                .ToListAsync());

        return Ok(result);
    }
}
```

### Extensions

You can write custom SQL syntax extensions and functions as you can see it in *[Extensions/CustomSqlExtensions.cs](Extensions/CustomSqlExtensions.cs)*.

For more examples check out [this article](http://blog.linq2db.com/2016/06/how-to-teach-linq-to-db-convert-custom.html).
