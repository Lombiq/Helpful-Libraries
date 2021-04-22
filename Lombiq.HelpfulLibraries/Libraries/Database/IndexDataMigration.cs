using OrchardCore.Data.Migration;
using YesSql.Indexes;

namespace Lombiq.HelpfulLibraries.Libraries.Database
{
    /// <summary>
    /// Use this as a normal <see cref="IDataMigration"/> for a single index. The generic type is considered a contract
    /// and doesn't actually do anything. Use it in case you need to synchronize type constraints in your code.
    /// </summary>
    /// <typeparam name="TIndex">The type of the <see cref="MapIndex"/> this migration is for.</typeparam>
    public class IndexDataMigration<TIndex> : DataMigration
        where TIndex : MapIndex
    {
    }
}
