# Lombiq Helpful Libraries - LINQ to DB Libraries for Orchard Core



## About

With the help of this project you can write LINQ expressions and run them with a [YesSql](https://github.com/sebastienros/yessql) `ISession` extension method to query from the DB instead of writing plain SQL queries. Uses the [LINQ to DB project](https://linq2db.github.io/).

You can watch a demo video of the project [here](https://www.youtube.com/watch?v=ldJOdCSsWJo).

For general details about and on using the Helpful Libraries see the [root Readme](../Readme.md).


## Documentation

Use the `LinqQueryAsync` ISession extension method for running LINQ syntax-based DB queries. Check out its documentation inline.

See [`LinqToDbSamplesController`](../Lombiq.HelpfulLibraries.Samples/Controllers/LinqToDbSamplesController.cs) for an example.

### Extensions

You can write custom SQL syntax extensions and functions as you can see it in *[Extensions/CustomSqlExtensions.cs](Extensions/CustomSqlExtensions.cs)*.

For more examples check out [this article](http://blog.linq2db.com/2016/06/how-to-teach-linq-to-db-convert-custom.html).
