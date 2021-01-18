using Dapper;
using OrchardCore.Queries.Sql;
using System;
using System.Collections;
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
        /// <returns>The result set of the query, rows mapped to <typeparamref name="TRow"/>.</returns>
        public static async Task<IEnumerable<TRow>> RawQueryAsync<TRow>(
            this ISession session,
            string sql,
            IDictionary<string, object> parameters = null,
            Func<(string ParsedQuery, IDbTransaction Transaction), Task<IEnumerable<TRow>>> queryExecutor = null)
        {
            var transaction = await session.DemandAsync();

            var parserResult = SqlParser.TryParse(
                sql,
                TransactionSqlDialectFactory.For(transaction),
                session.Store.Configuration.TablePrefix,
                parameters,
                out var query,
                out var messages);

            if (!parserResult)
            {
                throw new RawQueryException(
                    $"Error during parsing the query \"{sql}\" with the following messages: {Environment.NewLine}{string.Join(Environment.NewLine, messages)}",
                    messages);
            }

            return queryExecutor == null
                ? await transaction.Connection.QueryAsync<TRow>(query, transaction: transaction)
                : await queryExecutor((query, transaction));
        }

        /// <summary>
        /// Updates the Content value of a <see cref="Document"/> directly in the Document table. It won't alter the
        /// <see cref="Document"/>'s version and won't execute index provider's either. Should be used for maintenance
        /// purposes only.
        /// </summary>
        /// <param name="documentId">ID of the <see cref="Document"/> in the Document table.</param>
        /// <param name="entity">Object that needs to be serialized to the Content field of the Document table.</param>
        /// <returns><see langword="true" /> if the query updated an existing <see cref="Document"/> successfully.</returns>
        public static async Task<bool> UpdateDocumentDirectlyAsync(this ISession session, int documentId, object entity)
        {
            var transaction = await session.DemandAsync();
            var dialect = session.Store.Dialect;
            var content = session.Store.Configuration.ContentSerializer.Serialize(entity);

            var sql = @$"UPDATE {dialect.QuoteForTableName(session.Store.Configuration.TablePrefix + Store.DocumentTable)}
                SET {dialect.QuoteForColumnName("Content")} = @Content
                WHERE {dialect.QuoteForColumnName("Id")} = @Id";

            return await transaction.Connection.ExecuteAsync(sql, new { Id = documentId, Content = content }, transaction) > 0;
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
        public override IDictionary Data { get; }

        public RawQueryException(string message, IEnumerable<string> errorMessages)
            : base(message) =>
            Data = errorMessages
                .Select((errorMessage, index) => (errorMessage, index))
                .ToDictionary(messageItem => messageItem.index, messageItem => messageItem.errorMessage);
    }
}
