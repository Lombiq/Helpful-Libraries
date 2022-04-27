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
    // We have no control over where these fields are declared.
#pragma warning disable CA1810 // Initialize reference type static fields inline
    static LinqToDbQueryExecutor()
#pragma warning restore CA1810 // Initialize reference type static fields inline
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
    /// <returns>The output of the query.</returns>
    public static Task<TResult> LinqTableQueryAsync<TTable, TResult>(
        this ISession session,
        Func<ITable<TTable>, Task<TResult>> query)
        where TTable : class =>
        session.LinqQueryAsync(accessor => query(accessor.GetTable<TTable>()));

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
            // Using explicit string instead of LinqToDB.ProviderName.SqlServer because if the "System.Data.SqlClient"
            // provider will be used it will cause "Could not load type System.Data.SqlClient.SqlCommandBuilder"
            // exception. See: https://github.com/linq2db/linq2db/issues/2191#issuecomment-618450439.
            "SqlServer" => "Microsoft.Data.SqlClient",
            "Sqlite" => ProviderName.SQLite,
            "MySql" => ProviderName.MySql,
            "PostgreSql" => ProviderName.PostgreSQL,
            _ => throw new NotSupportedException("The provider name is not supported."),
        };
}
