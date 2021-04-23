using LinqToDB;
using System;

namespace Lombiq.HelpfulLibraries.LinqToDb.Extensions
{
    public static class CustomSqlExtensions
    {
        [Sql.Expression("SqlServer", "JSON_VALUE({0}, {1})", ServerSideOnly = true, InlineParameters = true)]
        [Sql.Expression("Sqlite", "json_extract({0}, {1})", ServerSideOnly = true, InlineParameters = true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Used in the Sql.Expression attribute.")]
        public static string JsonValue(object expression, string path) => throw new InvalidOperationException();
    }
}
