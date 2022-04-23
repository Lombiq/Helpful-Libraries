using System;
using System.Threading.Tasks;

namespace OrchardCore.Settings;

public static class SiteServiceExtensions
{
    /// <summary>
    /// Returns a tuple containing the page size from the site settings and the number of pages needed to fit <paramref
    /// name="totalCount"/> items.
    /// </summary>
    public static async Task<(int PageSize, int PageCount)> GetPaginationInfoAsync(
        this ISiteService siteService,
        int totalCount)
    {
        var pageSize = (await siteService.LoadSiteSettingsAsync()).PageSize;
        var pageCount = (int)Math.Ceiling(1.0 * totalCount / pageSize);

        return (PageSize: pageSize, PageCount: pageCount);
    }
}
