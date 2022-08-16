using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using System.Data.Common;

namespace Lombiq.HelpfulLibraries.LinqToDb;

public class LinqToDbConnection : DataConnection, ITableAccessor
{
    private readonly string _tablePrefix;

    public LinqToDbConnection(IDataProvider dataProvider, DbTransaction dbTransaction, string tablePrefix)
        : base(dataProvider, dbTransaction) =>
            _tablePrefix = tablePrefix;

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
