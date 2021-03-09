# LINQ to DB helper Documentation



## Extensions

`QueryAsync<TResult>(this ISession session, Func<ITableAccessor, IQueryable> query)`
ISession extension for running LINQ syntax based DB queries.

`QueryAsync<TFirst, TSecond, TResult>(
    this ISession session,
    Func<ITableAccessor, IQueryable> query,
    Func<TFirst, TSecond, TResult> map,
    string splitOn)` 
ISession extension for running LINQ syntax based DB queries,
and also mapping result to the given type and on the given splitter.


## Sample

### Simple queries
```
public class LinqToDbSamplesController
    {
        private readonly ISession _session;
        public LinqToDbSamplesController(ISession session) => _session = session;

        [Route("LinqToDbSample/SimpleQuery")]
        public async Task<ActionResult> SimpleQuery()
        {
            var result = await _session.QueryAsync<AutoroutePartIndex>(
                accessor => accessor
                    .GetTable<AutoroutePartIndex>()
                    .Where(index => index.Path.Contains("a", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(index => index.Path));

            return Ok(result);
        }

        [Route("LinqToDbSample/JoinQuery")]
        public async Task<ActionResult> JoinQuery()
        {
            var result = await _session.QueryAsync<string>(
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

### Query with mapping

```
IQueryable RawQuery(ITableAccessor accessor) =>
    from qContentItemIndex in accessor.GetTable<ContentItemIndex>()
    join qAutoroutePartIndex in accessor.GetTable<AutoroutePartIndex>()
        on qContentItemIndex.ContentItemId equals qAutoroutePartIndex.ContentItemId
    where qAutoroutePartIndex.Path.StartsWith("press-release/", StringComparison.OrdinalIgnoreCase)
    select new 
    {
        qContentItemIndex.ContentItemId,
        qContentItemIndex.DisplayText,
    });


await _session.QueryAsync<string, int, KeyValuePair<string, int>>(
    RawQuery,
    (key, value) => new KeyValuePair<string, int>(key, value),
    splitOn: "DisplayText")
```



