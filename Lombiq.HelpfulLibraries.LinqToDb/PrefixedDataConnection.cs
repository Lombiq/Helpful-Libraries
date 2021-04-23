using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;

namespace Lombiq.HelpfulLibraries.LinqToDb
{
    public class PrefixedDataConnection : DataConnection
    {
        public static string TablePrefix { get; set; }

        public PrefixedDataConnection(LinqToDbConnectionOptions<PrefixedDataConnection> options)
            : base(options) { }

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
