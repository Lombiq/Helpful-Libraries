using Dapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YesSql;
using YesSql.Indexes;

namespace Lombiq.HelpfulLibraries.Libraries.Database
{
    public class ManualConnectingIndexService<T> : IManualConnectingIndexService<T>
        where T : MapIndex
    {
        private readonly Type _type;
        private readonly Dictionary<string, PropertyInfo> _properties;
        private readonly ILogger<ManualConnectingIndexService<T>> _logger;
        private readonly string _keys;

        private string _documentIdKey;
        private string _columns;

        public ManualConnectingIndexService(
            ILogger<ManualConnectingIndexService<T>> logger)
        {
            _type = typeof(T);
            _properties = _type
                .GetProperties()
                .Where(property => property.Name != nameof(MapIndex.Id))
                .ToDictionary(property => property.Name);

            _logger = logger;
            _keys = string.Join(", ", _properties.Keys.Select(key => "@" + key));
        }

        public Task AddAsync(T item, ISession session, int? setDocumentId = null) =>
            RunTransactionAsync(session, async (connection, dialect, name) =>
            {
                _documentIdKey ??= dialect.QuoteForColumnName("DocumentId");
                _columns ??= string.Join(", ", _properties.Keys.Select(dialect.QuoteForColumnName));

                var documentId = setDocumentId ?? (item as IIndex).GetAddedDocuments().Single().Id;
                var sql = $"INSERT INTO {name} ({_documentIdKey}, {_columns}) VALUES ({dialect.GetSqlValue(documentId)}, {_keys});";

                try
                {
                    return await connection.ExecuteAsync(sql, item);
                }
                catch
                {
                    _logger.LogError(
                        "Failed to execute the following SQL query:\n{0}\nArguments:\n{1}",
                        sql,
                        JsonConvert.SerializeObject(item));
                    throw;
                }
            });

        public Task RemoveAsync(string columnName, object value, ISession session) =>
            RunTransactionAsync(session, (connection, dialect, name) =>
            connection.ExecuteAsync(
                $"DELETE FROM {name} WHERE {dialect.QuoteForColumnName(columnName)} = @value",
                new { value }));

        private async Task<TOut> RunTransactionAsync<TOut>(
            ISession session,
            Func<DbConnection, ISqlDialect, string, Task<TOut>> request)
        {
            var prefix = session.Store.Configuration.TablePrefix;
            var dialect = session.Store.Configuration.SqlDialect;
            var quotedTableName = dialect.QuoteForTableName(prefix + _type.Name);

            var connection = await session.CreateConnectionAsync();
            return await request(connection, dialect, quotedTableName);
        }
    }
}
