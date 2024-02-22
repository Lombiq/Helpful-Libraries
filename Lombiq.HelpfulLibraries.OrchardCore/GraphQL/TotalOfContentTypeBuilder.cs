using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentManagement.Records;
using System;
using YesSql;

namespace Lombiq.HelpfulLibraries.OrchardCore.GraphQL;

/// <summary>
/// Adds a "totalOfContentType" field to each top level <see cref="ContentItem"/> type node.
/// </summary>
public class TotalOfContentTypeBuilder : IContentTypeBuilder
{
    private readonly IStringLocalizer<TotalOfContentTypeBuilder> S;

    public TotalOfContentTypeBuilder(IStringLocalizer<TotalOfContentTypeBuilder> stringLocalizer) =>
        S = stringLocalizer;

    /// <summary>
    /// Adds the <c>totalOfContentType</c> integer field to the given content item type.
    /// </summary>
    /// <param name="contentTypeDefinition">The content type to query for and operate on.</param>
    /// <param name="contentItemType">
    /// The content item type to be extended with the <c>totalOfContentType</c> integer field.
    /// </param>
    public void Build(FieldType contentQuery, ContentTypeDefinition contentTypeDefinition, ContentItemType contentItemType)
    {
        var name = contentTypeDefinition.Name;

        var builder = contentItemType
            .Field<IntGraphType, int>("totalOfContentType")
            .Description(S["Gets the total count of all published content items with the type {0}.", name]);

        builder.ResolveAsync(async context =>
        {
            var session = context.RequestServices?.GetService<ISession>() ?? throw new InvalidOperationException(
                $"Couldn't retrieve {nameof(ISession)} service from {nameof(context)} of \"{name}\".");

            return await session.QueryIndex<ContentItemIndex>(index =>
                index.Published &&
                index.Latest &&
                index.ContentType == name)
                .CountAsync();
        });
    }
}
