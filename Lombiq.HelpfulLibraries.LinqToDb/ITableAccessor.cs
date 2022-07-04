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

    /// <summary>
    /// Returns a table for query building purposes.
    /// </summary>
    /// <typeparam name="T">Type of the object representing the table in the database.</typeparam>
    /// <param name="collectionName">
    /// Name of the YesSql collection that includes logically related objects. It is technically a prefix to the
    /// affected tables in the database.
    /// </param>
    /// <remarks>
    /// <para>
    /// The reason for overloading the GetTable method instead of adding <paramref name="collectionName"/> as an
    /// optional parameter is that otherwise we'd get an "error CS0854: An expression tree may not contain a call or
    /// invocation that uses optional arguments".
    /// </para>
    /// </remarks>
    /// <returns>Linq2db table for query building purposes.</returns>
    ITable<T> GetTable<T>(string collectionName)
        where T : class;
}
