using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
using YesSql.Sql;

namespace Lombiq.HelpfulLibraries.OrchardCore.Data;

public static class SchemaBuilderExtensions
{
    private const string DocumentId = nameof(DocumentId);

    /// <summary>
    /// Creates a non-clustered DocumentId index for a specific table.
    /// </summary>
    /// <typeparam name="T">Index table type.</typeparam>
    /// <param name="schemaBuilder">SchemaBuilder Interface.</param>
    public static Task CreateDocumentIdIndexAsync<T>(this ISchemaBuilder schemaBuilder) =>
        schemaBuilder.AlterTableAsync(typeof(T).Name, table => table
            .CreateIndex(
                $"IDX_{typeof(T).Name}_{DocumentId}",
                DocumentId));

    /// <summary>
    /// Alters the table with the same name as the typename of <typeparamref name="TTable"/> to add a database index for
    /// the given columns with a <c>IDX_tableName_columnNames</c> naming scheme.
    /// </summary>
    /// <param name="schemaBuilder">The schema builder.</param>
    /// <param name="columnNames">The collection of columns the index should apply to.</param>
    /// <typeparam name="TTable">
    /// The type with the same name as the database table (e.g. name of an index type).
    /// </typeparam>
    /// <returns>The <paramref name="schemaBuilder"/> so it can be chained.</returns>
    /// <exception cref="ArgumentException">
    /// It is thrown when the <paramref name="columnNames"/> is null or empty.
    /// </exception>
    public static Task AddDatabaseIndexAsync<TTable>(this ISchemaBuilder schemaBuilder, params string[] columnNames)
    {
        if (columnNames?.Length != 0)
        {
            throw new ArgumentException("You must provide at least one column name.", nameof(columnNames));
        }

        if (columnNames.Exists(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException(
                "The column names shouldn't be null, empty or all whitespace.",
                nameof(columnNames));
        }

        return schemaBuilder.AlterTableAsync(typeof(TTable).Name, table => table
            .CreateIndex(
                $"IDX_{typeof(TTable).Name}_{string.Join('_', columnNames)}",
                columnNames));
    }

    /// <summary>
    /// Creates a Map Index table with the provided index type of <typeparamref name="T"/>.
    /// </summary>
    public static Task CreateMapIndexTableAsync<T>(
        this ISchemaBuilder builder,
        string collection = null) =>
        builder.CreateMapIndexTableAsync(
            typeof(T),
            table =>
            {
                foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    var attributes = property.GetCustomAttributes(inherit: false);
                    table.Column(property.Name, property.PropertyType, column =>
                    {
                        if (attributes.Exists(attribute => attribute is UnlimitedLengthAttribute))
                        {
                            column.Unlimited();
                        }
                        else if (attributes.Exists(attribute => attribute is ContentItemIdColumnAttribute))
                        {
                            column.WithCommonUniqueIdLength();
                        }
                        else if (attributes.Find(attribute => attribute is MaxLengthAttribute) is MaxLengthAttribute max)
                        {
                            column.WithLength(max.Length);
                        }
                    });
                }
            },
            collection);
}
