using Dapper;
using LinqToDB;
using LinqToDB.Data;
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
        public static async Task<IEnumerable<TResult>> LinqQueryAsync<TResult>(
            this ISession session,
            Func<ITableAccessor, IQueryable> query)
        {
            var transaction = await session.DemandAsync();
            var convertedSql = ConvertSqlToDialect(session, transaction, query);

            return await transaction.Connection.QueryAsync<TResult>(convertedSql, transaction: transaction);
        }

        private static string ConvertSqlToDialect(
            ISession session,
            IDbTransaction transaction,
            Func<ITableAccessor, IQueryable> query)
        {
            // Generate aliases for final projection.
            LinqToDB.Common.Configuration.Sql.GenerateFinalAliases = true;

            // Instantiating a linq2db connection object as it is required to start building the query. Note that it
            // won't create an actual connection with the database yet.
            var dataProvider = DataConnection.GetDataProvider(
                GetDatabaseProviderName(session.Store.Dialect.Name),
                transaction.Connection.ConnectionString);

            using var linqToDbConnection = new LinqToDbConnection(
                dataProvider,
                transaction,
                session.Store.Configuration.TablePrefix);
            return query(linqToDbConnection).ToString();
        }

        private static string GetDatabaseProviderName(string dbName) =>
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
