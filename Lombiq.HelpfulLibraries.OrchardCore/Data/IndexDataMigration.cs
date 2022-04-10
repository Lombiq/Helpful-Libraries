using OrchardCore.Data.Migration;
using YesSql.Indexes;
using YesSql.Sql;
using YesSql.Sql.Schema;

namespace Lombiq.HelpfulLibraries.OrchardCore.Data;

/// <summary>
/// Use this as a normal <see cref="IDataMigration"/> for a single index. The generic type is considered a contract and
/// doesn't actually do anything. Use it in case you need to synchronize type constraints in your code.
/// </summary>
/// <typeparam name="TIndex">The type of the <see cref="MapIndex"/> this migration is for.</typeparam>
public abstract class IndexDataMigration<TIndex> : DataMigration
    where TIndex : MapIndex
{
    protected virtual int CreateVersion => 1;

    public int Create()
    {
        SchemaBuilder.CreateMapIndexTable<TIndex>(CreateIndex);

        SchemaBuilder.CreateDocumentIdIndex<TIndex>();

        return CreateVersion;
    }

    protected abstract void CreateIndex(ICreateTableCommand table);
}
