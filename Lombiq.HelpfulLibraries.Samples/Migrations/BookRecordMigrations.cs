using Lombiq.HelpfulLibraries.Samples.Models;
using OrchardCore.Data.Migration;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Samples.Migrations;

public sealed class BookRecordMigrations : DataMigration
{
    public async Task<int> CreateAsync()
    {
        await SchemaBuilder.CreateTableAsync(nameof(BookRecord), table => table
            .Column<int>(nameof(BookRecord.Id), column => column.PrimaryKey().Identity())
            .Column<string>(nameof(BookRecord.Title), column => column.NotNull().Unique().WithLength(2048))
            .Column<string>(nameof(BookRecord.Author), column => column.NotNull())
        );

        return 1;
    }
}
