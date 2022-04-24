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

    public ITable<T> GetPrefixedTable<T>()
        where T : class
    {
        var table = base.GetTable<T>();
        return table.TableName(_tablePrefix + table.TableName);
    }

    public new ITable<T> GetTable<T>()
        where T : class
            => GetPrefixedTable<T>();
}
