using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement;
using OrchardCore.Navigation;
using OrchardCore.Settings;
using System.Collections.Generic;
using System.Globalization;
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

    /// <summary>
    /// Gets the appropriate page of the <paramref name="query"/>, then returns its values, the total count and the
    /// <c>Pager</c> shape that can be used to navigate to other pages.
    /// </summary>
    /// <param name="query">The query that should be paged.</param>
    /// <param name="shapeFactory">Used to create the <c>Pager</c> shape.</param>
    /// <param name="siteService">
    /// If <paramref name="defaultPageSize"/> is <see langword="null"/>, this is used to get <see
    /// cref="ISite.PageSize"/> as the fallback value for the default page size. So if it's specified then this can be
    /// <see langword="null"/>.
    /// </param>
    /// <param name="pagerParameters">Used to configure the current page number and similar values.</param>
    /// <param name="routeData">
    /// The source of other route values that should be supplied to the pager links. Normally you should pass <see
    /// cref="HttpRequest.RouteValues"/> to this parameter.
    /// </param>
    /// <param name="defaultPageSize">
    /// An optional value it you want custom page size instead of the value coming from <see cref="ISite.PageSize"/>.
    /// </param>
    /// <typeparam name="T">The type of the items in the <paramref name="query"/>.</typeparam>
    /// <remarks><para>
    /// The <see cref="PagerParameters.Page"/> should come from the <c>pagenum</c> <!-- #spell-check-ignore-line -->
    /// route value. The number should be one-based.
    /// </para></remarks>
    public static async Task<GetPageAndPagerViewModel<T>> GetPageAndPagerAsync<T>(
        this IQuery<T> query,
        IShapeFactory shapeFactory,
        ISiteService siteService,
        PagerParameters pagerParameters,
        RouteValueDictionary routeData,
        int? defaultPageSize = null)
        where T : class
    {
        var pager = new Pager(pagerParameters, defaultPageSize ?? (await siteService.GetSiteSettingsAsync()).PageSize);
        var index = pager.GetStartIndex();

        var total = await query.CountAsync();
        var items = (await query.PaginateAsync(index, pager.PageSize)).AsList();

        var pagerShape = (await shapeFactory.New.Pager(pager))
            .TotalItemCount(total)
            .RouteData(routeData == null ? new RouteData() : new RouteData(routeData));

        return new GetPageAndPagerViewModel<T>(items, pagerShape, total, pager.PageSize, index);
    }

    /// <summary>
    /// Gets the appropriate page of the <paramref name="query"/>, then returns its values, the total count and the
    /// <c>Pager</c> shape that can be used to navigate to other pages.
    /// </summary>
    /// <param name="httpContext">Used to source some required services and current request information.</param>
    /// <param name="pageNumber">
    /// If not <see langword="null"/>, it's used to configure the <see cref="PagerParameters"/>. Otherwise the query
    /// value of <c>pagenum</c> is used from <paramref name="httpContext"/>. <!-- #spell-check-ignore-line -->
    /// </param>
    /// <param name="defaultPageSize">
    /// An optional value it you want custom page size instead of the value coming from <see cref="ISite.PageSize"/>.
    /// </param>
    public static Task<GetPageAndPagerViewModel<T>> GetPageAndPagerAsync<T>(
        this IQuery<T> query,
        HttpContext httpContext,
        int? pageNumber = null,
        int? defaultPageSize = null)
        where T : class
    {
        var page = pageNumber ?? 0;
        if (page <= 0)
        {
            page = httpContext.Request.Query.TryGetValue("pagenum", out var pageNumberString) && // #spell-check-ignore-line
                   int.TryParse(pageNumberString, CultureInfo.InvariantCulture, out var pageNumberInt)
                ? pageNumberInt
                : 1;
        }

        var provider = httpContext.RequestServices;

        return query.GetPageAndPagerAsync(
            provider.GetRequiredService<IShapeFactory>(),
            provider.GetRequiredService<ISiteService>(),
            new PagerParameters { Page = page > 0 ? page : 1 },
            httpContext.Request.RouteValues,
            defaultPageSize);
    }
}

public record GetPageAndPagerViewModel<T>(IList<T> Items, IShape Pager, int Total, int PageSize, int PageIndex);
