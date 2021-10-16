using System;
using System.Linq;
using YesSql.Sql;

namespace Lombiq.HelpfulLibraries.Libraries.Database
{
    public static class SchemaBuilderExtensions
    {
        private const string DocumentId = nameof(DocumentId);

        /// <summary>
        /// Creates a non-clustered DocumentId index for a specific table.
        /// </summary>
        /// <typeparam name="T">Index table type.</typeparam>
        /// <param name="schemaBuilder">SchemaBuilder Interface.</param>
        public static void CreateDocumentIdIndex<T>(this ISchemaBuilder schemaBuilder) =>
            schemaBuilder.AlterTable(typeof(T).Name, table => table
                .CreateIndex(
                    $"IDX_{typeof(T).Name}_{DocumentId}",
                    DocumentId));

        /// <summary>
        /// Alters the table with the same name as the typename of <typeparamref name="TTable"/> to add a database index
        /// for the given columns with a <c>IDX_tableName_columnNames</c> naming scheme.
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
        public static ISchemaBuilder AddDatabaseIndex<TTable>(this ISchemaBuilder schemaBuilder, params string[] columnNames)
        {
            if (columnNames?.Any() != true)
            {
                throw new ArgumentException($"{nameof(columnNames)} must contain at least one column name");
            }

            return schemaBuilder.AlterTable(nameof(TTable), table => table
                .CreateIndex(
                    $"IDX_{nameof(TTable)}_{string.Join("_", columnNames)}",
                    columnNames));
        }
    }
}
