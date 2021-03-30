using YesSql.Provider.Sqlite;
using YesSql.Provider.SqlServer;

namespace YesSql
{
    public static class SqlDialectExtensions
    {
        /// <summary>
        /// Returns <see langword="true"/> if the database uses Microsoft SQL Server's dialect of SQL.
        /// </summary>
        public static bool IsSqlServer(this ISqlDialect dialect) => dialect is SqlServerDialect;

        /// <summary>
        /// Returns <see langword="true"/> if the database uses SQLite's dialect of SQL.
        /// </summary>
        public static bool IsSqlite(this ISqlDialect dialect) => dialect is SqliteDialect;
    }
}
