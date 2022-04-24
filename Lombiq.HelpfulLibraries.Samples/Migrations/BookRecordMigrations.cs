using Lombiq.HelpfulLibraries.Samples.Models;
using OrchardCore.Data.Migration;

namespace Lombiq.HelpfulLibraries.Samples.Migrations;

public class BookRecordMigrations : DataMigration
{
    public int Create()
    {
        SchemaBuilder.CreateTable(nameof(BookRecord), table => table
            .Column<int>(nameof(BookRecord.Id), column => column.PrimaryKey().Identity())
            .Column<string>(nameof(BookRecord.Title), column => column.NotNull().Unique().WithLength(2048))
            .Column<string>(nameof(BookRecord.Author), column => column.NotNull())
        );

        return 1;
    }
}
