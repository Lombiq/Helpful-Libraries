using LinqToDB;
using LinqToDB.Data;

namespace Lombiq.HelpfulLibraries.LinqToDb
{
    public class PrefixedDataConnection : DataConnection, ITableAccessor
    {
        private readonly string _tablePrefix;

        public PrefixedDataConnection(string databaseName, string connectionString, string tablePrefix)
            : base(databaseName, connectionString) =>
            _tablePrefix = tablePrefix;

        public ITable<T> GetPrefixedTable<T>()
            where T : class
        {
            var table = GetTable<T>();
            return table.TableName(_tablePrefix + table.TableName);
        }

        ITable<T> ITableAccessor.GetTable<T>() => GetPrefixedTable<T>();
    }
}
