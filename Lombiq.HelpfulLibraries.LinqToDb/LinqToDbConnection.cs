using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using System.Data;

namespace Lombiq.HelpfulLibraries.LinqToDb;

public class LinqToDbConnection : DataConnection, ITableAccessor
{
    private readonly string _tablePrefix;

    public LinqToDbConnection(IDataProvider dataProvider, IDbTransaction dbTransaction, string tablePrefix)
        : base(dataProvider, dbTransaction) =>
            _tablePrefix = tablePrefix;

    public ITable<T> GetPrefixedTable<T>(string collectionName = null)
        where T : class
    {
        var table = base.GetTable<T>();

        var tableName = string.IsNullOrEmpty(collectionName)
            ? _tablePrefix + table.TableName
            : _tablePrefix + collectionName + "_" + table.TableName;

        return table.TableName(tableName);
    }

    public ITable<T> GetTable<T>(string collectionName = null)
        where T : class
            => GetPrefixedTable<T>(collectionName);
}
