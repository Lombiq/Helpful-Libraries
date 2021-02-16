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
    }
}
