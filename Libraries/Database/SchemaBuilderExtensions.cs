using YesSql.Sql;

namespace Lombiq.HelpfulLibraries.Libraries.Database
{
    public static class SchemaBuilderExtensions
    {
        private const string DocumentId = nameof(DocumentId);

        public static void CreateDocumentIdIndex<T>(this ISchemaBuilder schemaBuilder)
            where T : class => schemaBuilder.AlterTable(typeof(T).Name, table => table
                                 .CreateIndex(
                                     $"IDX_{typeof(T).Name}_{DocumentId}",
                                     DocumentId));
    }
}
