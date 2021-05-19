# LinqToDb - Lombiq HelpfulLibraries for Orchard Core



## About

With the help of this module you can write LINQ expressions to query from DB instead of plain SQL queries.


## Documentation

Use the `LinqQueryAsync` ISession extension for running LINQ syntax based DB queries.
```csharp 
LinqQueryAsync<TResult>(this ISession session,Func<ITableAccessor, IQueryable> query)
```

### Sample controller

```csharp
public class LinqToDbSamplesController
    {
        private readonly ISession _session;
        public LinqToDbSamplesController(ISession session) => _session = session;

        public async Task<ActionResult> SimpleQuery()
        {
            var result = await _session.LinqQueryAsync<AutoroutePartIndex>(
                accessor => accessor
                    .GetTable<AutoroutePartIndex>()
                    .Where(index => index.Path.Contains("a", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(index => index.Path));

            return Ok(result);
        }

        public async Task<ActionResult> JoinQuery()
        {
            var result = await _session.LinqQueryAsync<string>(
                accessor =>
                    from qContentItemIndex in accessor.GetTable<ContentItemIndex>()
                    join qAutoroutePartIndex in accessor.GetTable<AutoroutePartIndex>()
                        on qContentItemIndex.ContentItemId equals qAutoroutePartIndex.ContentItemId
                    where qAutoroutePartIndex.Path.StartsWith("press-release/", StringComparison.OrdinalIgnoreCase)
                    select qContentItemIndex.DisplayText);

            return Ok(result);
        }
    }
```

### Extensions

You can write custom SQL syntax extensions and functions as you can see it in *[Extensions\CustomSqlExtensions.cs](Extensions\CustomSqlExtensions.cs)*.

For more examples check out [this article](http://blog.linq2db.com/2016/06/how-to-teach-linq-to-db-convert-custom.html).

