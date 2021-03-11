using Dapper;
using LinqToDB;
using Lombiq.HelpfulLibraries.LinqToDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using YesSql;

namespace Lombiq.HelpfulLibraries.LinqToDb
{
    public static class OrchardCoreLinqToDbQueryExecutor
    {
        public static async Task<IEnumerable<TResult>> QueryAsync<TResult>(
            this ISession session,
            Func<ITableAccessor, IQueryable> query)
        {
            var transaction = await session.DemandAsync();
            var convertedSql = SetupAndConvertSqlToDialect(session, transaction, query);

            return await transaction.Connection.QueryAsync<TResult>(convertedSql, transaction: transaction);
        }

        public static async Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond, TResult>(
            this ISession session,
            Func<ITableAccessor, IQueryable> query,
            Func<TFirst, TSecond, TResult> map,
            string splitOn)
        {
            var transaction = await session.DemandAsync();
            var convertedSql = SetupAndConvertSqlToDialect(session, transaction, query);

            return await transaction.Connection.QueryAsync(convertedSql, map: map, transaction: transaction, splitOn: splitOn);
        }

        private static string SetupAndConvertSqlToDialect(
            ISession session,
            IDbTransaction transaction,
            Func<ITableAccessor, IQueryable> query)
        {
            // Generate aliases for final projection.
            LinqToDB.Common.Configuration.Sql.GenerateFinalAliases = true;

            // Instantiating a linq2db connection object as it is required to start building the query. Note that it
            // won't create an actual connection with the database yet.
            using var linqToDbConnection = new PrefixedDataConnection(
                GetDatabaseProvider(session.Store.Dialect.Name),
                transaction.Connection.ConnectionString,
                session.Store.Configuration.TablePrefix);

            return query(linqToDbConnection).ToString();
        }

        private static string GetDatabaseProvider(string dbName) =>
            dbName switch
            {
                // Using explicit string instead of LinqToDB.ProviderName.SQLite because the "System.Data.SqlClient"
                // provider will be used causing "Could not load type System.Data.SqlClient.SqlCommandBuilder"
                // exception. See: https://github.com/linq2db/linq2db/issues/2191#issuecomment-618450439
                "SqlServer" => "Microsoft.Data.SqlClient",
                "Sqlite" => ProviderName.SQLite,
                _ => throw new NotSupportedException("The provider name is not supported."),
            };
    }
}
