using OrchardCore.Taxonomies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrchardCore.ContentManagement
{
    public static class ContentManagerExtensions
    {
        public static async Task<T> GetAsync<T>(this IContentManager contentManager, string id)
            where T : ContentPart
            => (await contentManager.GetAsync(id))?.As<T>();

        public static async Task<T> GetAsync<T>(this IContentManager contentManager, string id, VersionOptions versionOptions)
            where T : ContentPart
            => (await contentManager.GetAsync(id, versionOptions))?.As<T>();

        public static Task CreateOrUpdateAsync(this IContentManager contentManager, ContentItem contentItem) =>
            contentItem.Id == 0
                ? contentManager.CreateAsync(contentItem, VersionOptions.Published)
                : contentManager.UpdateAsync(contentItem);

        public static Task<ContentItem> NewOrLoadAsync(
            this IContentManager contentManager,
            ContentItem contentItem,
            string name) =>
            contentItem == null ? contentManager.NewAsync(name) : contentManager.LoadAsync(contentItem);

        public static async Task<List<ContentItem>> GetTaxonomyTermsAsync(
            this IContentManager contentManager,
            IContentHandleManager contentHandleManager,
            string taxonomyAlias)
        {
            var taxonomyContentItemId = await contentHandleManager.GetContentItemIdAsync($"alias:{taxonomyAlias}");
            var taxonomy = await contentManager.GetAsync(taxonomyContentItemId);

            return taxonomy.As<TaxonomyPart>().Terms;
        }
    }
}
