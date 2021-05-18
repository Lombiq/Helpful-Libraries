using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using System.Data;

namespace Lombiq.HelpfulLibraries.LinqToDb
{
    public class LinqToDbConnection : DataConnection, ITableAccessor
    {
        public string TablePrefix { get; set; }

        public LinqToDbConnection(IDataProvider dataProvider, IDbTransaction dbTransaction, string tablePrefix)
            : base(dataProvider, dbTransaction) =>
                TablePrefix = tablePrefix;

        public ITable<T> GetPrefixedTable<T>()
            where T : class
        {
            var table = base.GetTable<T>();
            return table.TableName(TablePrefix + table.TableName);
        }

        public new ITable<T> GetTable<T>()
            where T : class
                => GetPrefixedTable<T>();
    }
}
