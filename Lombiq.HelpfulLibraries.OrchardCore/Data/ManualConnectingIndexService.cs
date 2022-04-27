using Dapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrchardCore.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YesSql;
using YesSql.Indexes;

namespace Lombiq.HelpfulLibraries.OrchardCore.Data;

public class ManualConnectingIndexService<T> : IManualConnectingIndexService<T>
    where T : MapIndex
{
    private readonly Type _type;
    private readonly Dictionary<string, PropertyInfo> _properties;
    private readonly IDbConnectionAccessor _dbAccessor;
    private readonly ILogger _logger;
    private readonly string _keys;

    private string _documentIdKey;
    private string _columns;
    private string _tablePrefix;

    public ManualConnectingIndexService(
        IDbConnectionAccessor dbAccessor,
        ILogger<ManualConnectingIndexService<T>> logger)
    {
        _type = typeof(T);
        _properties = _type
            .GetProperties()
            .Where(property => property.Name != nameof(MapIndex.Id))
            .ToDictionary(property => property.Name);

        _dbAccessor = dbAccessor;
        _logger = logger;
        _keys = string.Join(", ", _properties.Keys.Select(key => "@" + key));
    }

    public Task AddAsync(T item, ISession session, int? setDocumentId = null) =>
        RunTransactionAsync(session, async (connection, transaction, dialect, name) =>
        {
            _documentIdKey ??= dialect.QuoteForColumnName("DocumentId");
            _columns ??= string.Join(", ", _properties.Keys.Select(dialect.QuoteForColumnName));

            var documentId = setDocumentId ?? (item as IIndex).GetAddedDocuments().Single().Id;
            var sql = $"INSERT INTO {name} ({_documentIdKey}, {_columns}) VALUES ({dialect.GetSqlValue(documentId)}, {_keys});";

            try
            {
                return await connection.ExecuteAsync(sql, item, transaction);
            }
            catch
            {
                _logger.LogError(
                    "Failed to execute the following SQL query:\n{Sql}\nArguments:\n{Item}",
                    sql,
                    JsonConvert.SerializeObject(item));
                throw;
            }
        });

    public Task RemoveAsync(string columnName, object value, ISession session) =>
        RunTransactionAsync(session, (connection, transaction, dialect, name) =>
        connection.ExecuteAsync(
            $"DELETE FROM {name} WHERE {dialect.QuoteForColumnName(columnName)} = @value",
            new { value },
            transaction));

    private async Task<TOut> RunTransactionAsync<TOut>(
        ISession session,
        Func<DbConnection, DbTransaction, ISqlDialect, string, Task<TOut>> request)
    {
        async Task<TOut> Run(
            DbTransaction transaction,
            bool doCommit,
            Func<DbConnection, DbTransaction, ISqlDialect, string, Task<TOut>> request)
        {
            _tablePrefix ??= session?.Store.Configuration.TablePrefix;
            var dialect = session?.Store.Configuration.SqlDialect;
            var quotedTableName = dialect?.QuoteForTableName(_tablePrefix + _type.Name);

            var result = await request(transaction.Connection, transaction, dialect, quotedTableName);
            if (doCommit) await transaction.CommitAsync();
            return result;
        }

        if (session != null) return await Run(await session.BeginTransactionAsync(), doCommit: false, request);

        await using var connection = _dbAccessor.CreateConnection();
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        return await Run(transaction, doCommit: true, request);
    }
}
