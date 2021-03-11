using LinqToDB;
using System;

namespace Lombiq.HelpfulLibraries.Linq2Db
{
    public static class CustomSqlExtensions
    {
        [Sql.Expression("SqlServer", "JSON_VALUE({0}, {1})", ServerSideOnly = true, InlineParameters = true)]
        [Sql.Expression("Sqlite", "json_extract({0}, {1})", ServerSideOnly = true, InlineParameters = true)]
        public static string JsonValue(object expression, string path) => throw new InvalidOperationException();
    }
}
