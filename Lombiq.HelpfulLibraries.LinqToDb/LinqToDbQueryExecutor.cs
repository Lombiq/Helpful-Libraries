using LinqToDB;
using LinqToDB.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using YesSql;
using static LinqToDB.Common.Configuration;
using Sql = LinqToDB.Common.Configuration.Sql;

namespace Lombiq.HelpfulLibraries.LinqToDb;

public static class LinqToDbQueryExecutor
{
    static LinqToDbQueryExecutor()
    {
        // Generate aliases for final projection.
        Sql.GenerateFinalAliases = true;

        // We need to disable null comparison for joins. Otherwise it would generate a syntax like this:
        // JOIN Table2 ON Table1.Key = Table2.Key OR Table1.Key IS NULL AND Table2.Key IS NULL
        Linq.CompareNullsAsValues = false;
    }

    /// <summary>
    /// Run LINQ syntax-based DB queries on the current DB connection with LINQ to DB, on a single table.
    /// </summary>
    /// <typeparam name="TTable">The type of the table to query on.</typeparam>
    /// <typeparam name="TResult">The type of results to return.</typeparam>
    /// <param name="session">A YesSql session whose connection is used instead of creating a new one.</param>
    /// <param name="query">The <see cref="IQueryable"/> which will be run as a DB query.</param>
    /// <param name="collectionName">
    /// Name of the YesSql collection that includes logically related objects. It is technically a prefix of the names
    /// of the affected tables in the database.
    /// </param>
    /// <returns>The output of the query.</returns>
    public static Task<TResult> LinqTableQueryAsync<TTable, TResult>(
        this ISession session,
        Func<ITable<TTable>, Task<TResult>> query,
        string collectionName = null)
        where TTable : class =>
        session.LinqQueryAsync(accessor => query(accessor.GetTable<TTable>(collectionName)));

    /// <summary>
    /// Run LINQ syntax-based DB queries on the current DB connection with LINQ to DB.
    /// </summary>
    /// <typeparam name="TResult">The type of results to return.</typeparam>
    /// <param name="session">A YesSql session whose connection is used instead of creating a new one.</param>
    /// <param name="query">The <see cref="IQueryable"/> which will be run as a DB query.</param>
    /// <returns>The output of the query.</returns>
    /// <remarks>
    /// <para>
    /// The API uses a function to execute the query so we can handle disposing <see cref="LinqToDbConnection"/> too.
    /// </para>
    /// </remarks>
    public static async Task<TResult> LinqQueryAsync<TResult>(
        this ISession session,
        Func<ITableAccessor, Task<TResult>> query)
    {
        var transaction = await session.BeginTransactionAsync();

        // Instantiating a LinqToDB connection object as it is required to start building the query. Note that it won't
        // create an actual connection with the database.
        var dataProvider = DataConnection.GetDataProvider(
            GetDatabaseProviderName(session.Store.Configuration.SqlDialect.Name),
            transaction.Connection.ConnectionString);

        using var linqToDbConnection = new LinqToDbConnection(
            dataProvider,
            transaction,
            session.Store.Configuration.TablePrefix);

        return await query(linqToDbConnection);
    }

    private static string GetDatabaseProviderName(string dbName) =>
        dbName switch
        {
            // Using a concrete SQL Server version here removes the need for an initial auto-detection query that can
            // fail, see https://github.com/Lombiq/Helpful-Libraries/issues/236. This needs to be the same minimum
            // version YesSql and thus OC supports. You can check it out in YesSql's build workflow:
            // https://github.com/sebastienros/yessql/blob/main/.github/workflows/build.yml. Keep this up-to-date with
            // Orchard Core upgrades bringing YesSql upgrades.
            "SqlServer" => ProviderName.SqlServer2019,
            "Sqlite" => ProviderName.SQLite,
            "MySql" => ProviderName.MySql,
            "PostgreSql" => ProviderName.PostgreSQL,
            _ => throw new NotSupportedException("The provider name is not supported."),
        };
}
