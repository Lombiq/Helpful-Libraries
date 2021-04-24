using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;

namespace Lombiq.HelpfulLibraries.LinqToDb
{
    public class LinqToDbConnection : DataConnection
    {
        public static string TablePrefix { get; set; }

        public LinqToDbConnection(LinqToDbConnectionOptions<LinqToDbConnection> options)
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
