using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using System.Data.Common;

namespace Lombiq.HelpfulLibraries.LinqToDb;

public class LinqToDbConnection(
    IDataProvider dataProvider,
    DbTransaction dbTransaction,
    string tablePrefix) : DataConnection(dataProvider, dbTransaction), ITableAccessor
{
    private readonly string _tablePrefix = tablePrefix;

    /// <summary>
    /// For the current query, overrides <see cref="ITable{T}.TableName"/> of the table-like source
    /// <typeparamref name="T"/> used in the query with a prefixed table name that optionally includes the specified
    /// <paramref name="collectionName"/>.
    /// </summary>
    /// <returns>The original table-like object but with a prefixed table name.</returns>
    public ITable<T> GetPrefixedTable<T>(string collectionName = null)
        where T : class
    {
        var table = DataExtensions.GetTable<T>(this);

        var tableName = string.IsNullOrEmpty(collectionName)
            ? _tablePrefix + table.TableName
            : _tablePrefix + collectionName + "_" + table.TableName;

        return table.TableName(tableName);
    }

    public ITable<T> GetTable<T>()
        where T : class
            => GetPrefixedTable<T>();

    public ITable<T> GetTable<T>(string collectionName)
        where T : class
            => GetPrefixedTable<T>(collectionName);
}
