using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using System.Linq;
using System.Threading.Tasks;
using YesSql;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

public class ContentVersionNumberService(ISession session) : IContentVersionNumberService
{
    public Task<int> GetLatestVersionNumberAsync(string contentItemId) =>
        session.Query<ContentItem, ContentItemIndex>()
            .Where(index => index.ContentItemId == contentItemId)
            .CountAsync();

    public async Task<int> GetCurrentVersionNumberAsync(string contentItemId, string contentItemVersionId)
    {
        var versions = (await session.Query<ContentItem, ContentItemIndex>()
                .Where(index => index.ContentItemId == contentItemId)
                .OrderByDescending(index => index.CreatedUtc)
                .ListAsync())
            .ToList();

        return versions.FindLastIndex(version => version.ContentItemVersionId == contentItemVersionId) + 1;
    }
}
