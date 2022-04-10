using LinqToDB;

namespace Lombiq.HelpfulLibraries.LinqToDb;

/// <summary>
/// Abstraction that retrieves a table for query building purposes.
/// </summary>
public interface ITableAccessor
{
    /// <summary>
    /// Returns a table for query building purposes.
    /// </summary>
    /// <typeparam name="T">Type of the object representing the table in the database.</typeparam>
    /// <returns>Linq2db table for query building purposes.</returns>
    ITable<T> GetTable<T>()
        where T : class;
}
