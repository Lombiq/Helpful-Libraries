using Dapper;
using OrchardCore.Queries.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace YesSql
{
    public static class SessionExtensions
    {
        /// <summary>
        /// Executes a raw SQL string query in a database-agnostic way by running it through Orchard's <see
        /// cref="SqlParser"/>.
        /// </summary>
        /// <typeparam name="TRow">Type used to map the returned rows to.</typeparam>
        /// <param name="sql">
        /// The raw SQL string. Doesn't need to use table prefixes or care about SQL dialects.
        /// </param>
        /// <param name="parameters">Input parameters passed to the query.</param>
        /// <param name="queryExecutor">
        /// Delegate to execute a query in a custom way, based on the prepared inputs.
        /// </param>
        /// <param name="transaction">If not <see langword="null"/> it must be an open DB transaction.</param>
        /// <returns>The result set of the query, rows mapped to <typeparamref name="TRow"/>.</returns>
        public static async Task<IEnumerable<TRow>> RawQueryAsync<TRow>(
            this ISession session,
            string sql,
            IDictionary<string, object> parameters = null,
            Func<(string ParsedQuery, IDbTransaction Transaction), Task<IEnumerable<TRow>>> queryExecutor = null,
            DbTransaction transaction = null)
        {
            transaction ??= await session.DemandAsync();
            var query = GetQuery(sql, transaction, session, parameters);

            return queryExecutor == null
                ? await transaction.Connection.QueryAsync<TRow>(query, transaction: transaction)
                : await queryExecutor((query, transaction));
        }

        /// <summary>
        /// Executes a raw SQL string command that doesn't return data in a database-agnostic way by running it
        /// through Orchard's <see cref="SqlParser"/>.
        /// </summary>
        /// <param name="sql">
        /// The raw SQL string. Doesn't need to use table prefixes or care about SQL dialects.
        /// </param>
        /// <param name="parameters">Input parameters passed to the query.</param>
        /// <param name="transaction">If not <see langword="null"/> it must be an open DB transaction.</param>
        /// <returns>The number of rows affected.</returns>
        public static async Task<int> RawExecuteAsync(
            this ISession session,
            string sql,
            IDictionary<string, object> parameters = null,
            DbTransaction transaction = null)
        {
            transaction ??= await session.DemandAsync();
            var query = GetQuery(sql, transaction, session, parameters);

            return await transaction.Connection.ExecuteAsync(query, transaction: transaction);
        }

        private static string GetQuery(
            string sql,
            DbTransaction transaction,
            ISession session,
            IDictionary<string, object> parameters)
        {
            var parserResult = SqlParser.TryParse(
                sql,
                TransactionSqlDialectFactory.For(transaction),
                session.Store.Configuration.TablePrefix,
                parameters,
                out var query,
                out var messages);
            if (parserResult) return query;

            var messagesList = messages is IList<string> list ? list : messages.ToList();

            throw new RawQueryException(
                $"Error during parsing the query \"{sql}\" with the following messages: {Environment.NewLine}" +
                $"{string.Join(Environment.NewLine, messagesList)}",
                messagesList);
        }
    }

    // We could use SqlException but that doesn't have a ctor for messages.
    [SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "The exception is used in a very particular single case.")]
    [SuppressMessage(
        "Major Code Smell",
        "S3925:\"ISerializable\" should be implemented correctly",
        Justification = "There's no need to make this class serializable.")]
    public class RawQueryException : DbException
    {
        public override System.Collections.IDictionary Data { get; }

        public RawQueryException(string message, IEnumerable<string> errorMessages)
            : base(message) =>
            Data = errorMessages
                .Select((errorMessage, index) => (errorMessage, index))
                .ToDictionary(messageItem => messageItem.index, messageItem => messageItem.errorMessage);
    }
}
