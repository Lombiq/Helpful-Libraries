using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Html.Models;
using OrchardCore.Html.ViewModels;
using System;

namespace OrchardCore.DisplayManagement.Handlers;

public static class DriverExtensions
{
    public static ShapeResult RawHtml(this DisplayDriverBase driver, string html) =>
        driver.Initialize<HtmlBodyPartViewModel>(nameof(HtmlBodyPart), model =>
        {
            model.Html = html;
            model.ContentItem = new ContentItem { ContentType = nameof(RawHtml) };
            model.HtmlBodyPart = new HtmlBodyPart { Html = model.Html, ContentItem = model.ContentItem };
            model.TypePartDefinition = new ContentTypePartDefinition(
                nameof(RawHtml),
                new ContentPartDefinition(nameof(RawHtml), Array.Empty<ContentPartFieldDefinition>(), []),
                []);
        });
}
