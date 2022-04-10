using LinqToDB;
using System.Diagnostics.CodeAnalysis;

namespace Lombiq.HelpfulLibraries.LinqToDb.Extensions;

public static class CustomSqlExtensions
{
    // You don't need an actual implementation in the function body for these simple expressions. The parameters will be
    // used by the attribute.
    [Sql.Expression(ProviderName.SqlServer, "JSON_VALUE({0}, {1})", ServerSideOnly = true, InlineParameters = true)]
    [Sql.Expression(ProviderName.SQLite, "json_extract({0}, {1})", ServerSideOnly = true, InlineParameters = true)]
    [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Used in the Sql.Expression attribute.")]
    public static string JsonValue(object expression, string path) => null;

    [Sql.Expression(ProviderName.SqlServer, "JSON_MODIFY({0}, {1}, {2})", ServerSideOnly = true, InlineParameters = true)]
    [Sql.Expression(ProviderName.SQLite, "json_replace({0}, {1}, {2})", ServerSideOnly = true, InlineParameters = true)]
    [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Used in the Sql.Expression attribute.")]
    public static string JsonModify(string json, string path, string newValue) => null;

    [Sql.Expression(ProviderName.SqlServer, "JSON_QUERY({0})", ServerSideOnly = true, InlineParameters = true)]
    [Sql.Expression(ProviderName.SQLite, "json({0})", ServerSideOnly = true, InlineParameters = true)]
    [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Used in the Sql.Expression attribute.")]
    public static string JsonQuery(string value) => null;
}
