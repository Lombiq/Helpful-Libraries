using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;

namespace Lombiq.HelpfulLibraries.Linq2Db
{
    public class PrefixedDataConnection : DataConnection, ITableAccessor
    {
        public static string TablePrefix { get; set; }

        public PrefixedDataConnection(LinqToDbConnectionOptions<PrefixedDataConnection> options)
            : base(options) { }

        public ITable<T> GetPrefixedTable<T>()
            where T : class
        {
            var table = GetTable<T>();
            return table.TableName(TablePrefix + table.TableName);
        }

        ITable<T> ITableAccessor.GetTable<T>() => GetPrefixedTable<T>();
    }
}
