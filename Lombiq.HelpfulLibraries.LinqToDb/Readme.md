# Lombiq LinqToDb - Lombiq HelpfulLibraries for Orchard Core



## About

LINQ to DB is the fastest LINQ database access library offering a simple, light, fast, and type-safe layer between your POCO objects and your database.
Read more about this [library here](https://github.com/linq2db/linq2db).


## Documentation

Enable `Lombiq.HelpfulLibraries.LinqToDb` feature and use dependency injection for the data connection.
Keep in mind, that the LinqToDbConnection object is [disposable](https://linq2db.github.io/articles/general/Managing-data-connection.html)!
```csharp
public class LinqToDbSamplesController
    {
        private readonly LinqToDbConnection _db;
        public LinqToDbSamplesController(LinqToDbConnection db) => _db = db;

        public async Task<ActionResult> SimpleQuery()
        {
            using (_db) {                
                var query = _db
                    .GetTable<AutoroutePartIndex>()
                    .Where(index => index.Path.Contains("a", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(index => index.Path);
                return await query.ToListAsync();
            }
        }

        public async Task<ActionResult> JoinQueryWithQuerySintax()
        {
            using (_db) {
                var query = from qContentItemIndex in _db.GetTable<ContentItemIndex>()
                join qAutoroutePartIndex in _db.GetTable<AutoroutePartIndex>()
                    on qContentItemIndex.ContentItemId equals qAutoroutePartIndex.ContentItemId
                where qAutoroutePartIndex.Path.StartsWith("press-release/", StringComparison.OrdinalIgnoreCase)
                select qContentItemIndex.DisplayText);
                return await query.ToListAsync();
            }
        }
    }
```

### Extensions

You can write custom SQL syntax extensions and functions as you can see it in `Extensions\CustomSqlExtensions.cs`.

For more examples check out [this article](http://blog.linq2db.com/2016/06/how-to-teach-linq-to-db-convert-custom.html).

