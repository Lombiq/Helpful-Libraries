using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql.Indexes;

namespace YesSql;

public static class QueryExtensions
{
    /// <summary>
    /// Breaks the query up into pages and lists the page using the given zero-based index. If pageIndex is 0 and count
    /// is <see cref="int.MaxValue"/> then the whole query is listed.
    /// </summary>
    /// <param name="query">The query to paginate.</param>
    /// <param name="pageIndex">Zero-based index of the desired page.</param>
    /// <param name="count">The page size.</param>
    /// <returns>The desired page of the resulting items.</returns>
    public static Task<IEnumerable<T>> PaginateAsync<T>(
        this IQuery<T> query,
        int pageIndex = 0,
        int count = int.MaxValue)
        where T : class
    {
        if (pageIndex > 0) query = query.Skip(pageIndex * count);
        if (count < int.MaxValue) query = query.Take(count);
        return query.ListAsync();
    }

    /// <summary>
    /// Breaks the query up into pages and lists the page using the given zero-based index. If pageIndex is 0 and count
    /// is <see cref="int.MaxValue"/> then the whole query is listed.
    /// </summary>
    /// <typeparam name="TPart">
    /// The final results are converted into the <see cref="ContentPart"/> of this type.
    /// </typeparam>
    /// <param name="query">The query to paginate.</param>
    /// <param name="pageIndex">Zero-based index of the desired page.</param>
    /// <param name="count">The page size.</param>
    /// <returns>
    /// The desired page of the resulting <see cref="ContentItem"/>s converted into the desired ContentPart. Those that
    /// don't have it are discarded.
    /// </returns>
    public static Task<IEnumerable<TPart>> PaginateAsync<TPart>(
        this IQuery<ContentItem> query,
        int pageIndex = 0,
        int count = int.MaxValue)
        where TPart : ContentPart =>
        PaginateAsync(query, pageIndex, count).ContinueWith(t => t.Result.As<TPart>().Where(part => part != null), TaskScheduler.Default);

    /// <summary>
    /// Breaks the query up into pages and lists the page using the given zero-based index. If pageIndex is 0 and count
    /// is <see cref="int.MaxValue"/> then the whole query is listed.
    /// </summary>
    public static Task<IEnumerable<TIndex>> PaginateAsync<TIndex>(
        this IQueryIndex<TIndex> query,
        int pageIndex = 0,
        int count = int.MaxValue)
        where TIndex : IIndex =>
        query.Skip(pageIndex * count).Take(count).ListAsync();

    /// <summary>
    /// Breaks the query up into slices and lists the slice.
    /// </summary>
    /// <param name="query">The query to slice.</param>
    /// <param name="skip">Number of items to skip. Can be null.</param>
    /// <param name="count">Number of items to take. Can be null.</param>
    /// <returns>The desired slices of the resulting <see cref="ContentItem"/>s.</returns>
    public static Task<IEnumerable<ContentItem>> SliceAsync(this IQuery<ContentItem> query, int? skip, int? count)
    {
        if (skip > 0) query = query.Skip(skip.Value);
        if (count > 0) query = query.Take(count.Value);

        return query.ListAsync();
    }

    /// <summary>
    /// A more compact shortcut for the <see cref="IQuery{T, TIndex}"/> ordering methods.
    /// </summary>
    /// <param name="query">The query to be ordered.</param>
    /// <param name="sql">The column name or other expression that may be put in the WHERE clause.</param>
    /// <param name="isAscending">
    /// If <see langword="true"/>, <see cref="IQuery{T,TIndex}.OrderBy(string)"/> or <see
    /// cref="IQuery{T,TIndex}.ThenBy(string)"/> is used, otherwise their <c>Descending</c> counterparts.
    /// </param>
    /// <param name="isFirstClause">
    /// If <see langword="true"/>, additional sorting expression is added. If <see langword="false"/> the primary
    /// sorting expression is set or overwritten.
    /// </param>
    /// <typeparam name="T">The query's item type after listing.</typeparam>
    /// <typeparam name="TIndex">The index used for sorting.</typeparam>
    /// <returns>An ordered query.</returns>
    public static IQuery<T, TIndex> OrderBy<T, TIndex>(
        this IQuery<T, TIndex> query,
        string sql,
        bool isAscending,
        bool isFirstClause = true)
        where T : class
        where TIndex : IIndex
    {
        if (isFirstClause) return isAscending ? query.OrderBy(sql) : query.OrderByDescending(sql);

        return isAscending ? query.ThenBy(sql) : query.ThenByDescending(sql);
    }
}
