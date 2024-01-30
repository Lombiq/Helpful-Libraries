using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using System.Linq;
using System.Threading.Tasks;
using YesSql;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

public class ContentVersionNumberService : IContentVersionNumberService
{
    private readonly ISession _session;

    public ContentVersionNumberService(ISession session) => _session = session;

    public Task<int> GetLatestVersionNumberAsync(string contentItemId) =>
        _session.Query<ContentItem, ContentItemIndex>()
            .Where(index => index.ContentItemId == contentItemId)
            .CountAsync();

    public async Task<int> GetCurrentVersionNumberAsync(string contentItemId, string contentItemVersionId)
    {
        var versions = (await _session.Query<ContentItem, ContentItemIndex>()
                .Where(index => index.ContentItemId == contentItemId)
                .OrderByDescending(index => index.CreatedUtc)
                .ListAsync())
            .ToList();

        return versions.FindLastIndex(version => version.ContentItemVersionId == contentItemVersionId) + 1;
    }
}
