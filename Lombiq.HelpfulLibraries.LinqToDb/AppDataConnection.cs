using LinqToDB.Configuration;
using LinqToDB.Data;

namespace Lombiq.HelpfulLibraries.Linq2Db
{
    public class AppDataConnection : DataConnection
    {
        public AppDataConnection(LinqToDbConnectionOptions<AppDataConnection> options)
            : base(options) { }
    }
}
