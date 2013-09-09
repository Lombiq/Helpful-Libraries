using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.ContentManagement;

namespace Piedone.HelpfulLibraries.Libraries.Contents
{
    /// <summary>
    /// PrefixedEditorManager is a contribution of Onestop Internet, Inc. (http://www.onestop.com/).
    /// </summary>
    public interface IPrefixedEditorManager : IDependency
    {
        IEnumerable<int> ItemIds { get; }
        dynamic BuildShape(IContent content, Func<IContent, dynamic> shapeFactory);
        dynamic UpdateEditor(IContent content, IUpdateModel updater, string groupId = "");
    }


    public static class PrefixedEditorManagerExtensions
    {
        public static IEnumerable<dynamic> BuildShapes(this IPrefixedEditorManager prefixedEditorManager, IEnumerable<ContentItem> contentItems, Func<IContent, dynamic> shapeFactory)
        {
            return contentItems.Select(item => prefixedEditorManager.BuildShape(item, shapeFactory)); ;
        }

        public static void UpdateEditors(this IPrefixedEditorManager prefixedEditorManager, IEnumerable<ContentItem> contentItems, IUpdateModel updater, string groupId = "")
        {
            foreach (var contentItem in contentItems)
            {
                prefixedEditorManager.UpdateEditor(contentItem, updater, groupId);
            }
        }
    }
}
