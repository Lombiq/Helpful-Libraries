using OrchardCore.ContentManagement;
using System.Threading.Tasks;
using YesSql;
using YesSql.Indexes;

namespace Lombiq.HelpfulLibraries.OrchardCore.Data;

/// <summary>
/// An alternative to <see cref="IndexProvider{T}"/> for manually creating connecting indices without the use of a
/// connecting document. When representing many-to-many connections, the user must decide which it resolves to when
/// calling <see cref="AddAsync"/>.
/// </summary>
/// <typeparam name="T">The index to be managed.</typeparam>
/// <remarks>
/// <list type="number">
///     <listheader><description>Usage</description></listheader>
///     <item><description>Create a <see cref="MapIndex"/> as usual, but make no <see cref="IndexProvider{T}"/>.</description></item>
///     <item><description>Register this class for the index as singleton.</description></item>
///     <item><description>Manually create the indices with <see cref="AddAsync"/>.</description></item>
///     <item><description>Look up the results using <see cref="IQuery"/> as usual.</description></item>
/// </list>
/// </remarks>
public interface IManualConnectingIndexService<in T>
    where T : MapIndex
{
    /// <summary>
    /// Adds a new entry to the index that refers to the document with the id in <paramref name="setDocumentId"/>. If
    /// that's <see langword="null"/> then the <see cref="Document"/> of the <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The index object to be recorded.</param>
    /// <param name="setDocumentId">
    /// If not <see langword="null"/>, overrides the index that may be in <paramref name="item"/>. It's also more
    /// convenient than casting it into <see cref="IIndex"/> to use <see cref="IIndex.AddDocument"/> when the index is
    /// created just on call.
    /// </param>
    /// <param name="session">If not null, its connection and transaction is used instead of creating a new one.</param>
    Task AddAsync(T item, ISession session, int? setDocumentId = null);

    /// <summary>
    /// Removes one or more existing indices using a standard SQL query where the given column has the given <paramref
    /// name="value"/>.
    /// </summary>
    /// <param name="columnName">The name of the column (i.e. the property name) to check.</param>
    /// <param name="value">The value to select for.</param>
    /// <param name="session">If not null, its connection and transaction is used instead of creating a new one.</param>
    Task RemoveAsync(string columnName, object value, ISession session);
}

public static class ManualConnectingIndexServiceExtensions
{
    public static Task RemoveByIndexAsync<T>(
        this IManualConnectingIndexService<T> service,
        int documentId,
        ISession session)
        where T : MapIndex =>
        service.RemoveAsync("DocumentId", documentId, session);

    public static Task RemoveByContentAsync<T>(
        this IManualConnectingIndexService<T> service,
        IContent content,
        ISession session)
        where T : MapIndex =>
        RemoveByIndexAsync(service, content.ContentItem.Id, session);
}
