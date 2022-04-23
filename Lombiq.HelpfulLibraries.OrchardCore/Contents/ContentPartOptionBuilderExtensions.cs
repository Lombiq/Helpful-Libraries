using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data.Migration;
using YesSql.Indexes;

namespace OrchardCore.ContentManagement;

public static class ContentPartOptionBuilderExtensions
{
    /// <summary>
    /// Adds an <see cref="IIndexProvider"/> to the service collection while maintaining the call chain for the content
    /// part builder.
    /// </summary>
    public static ContentPartOptionBuilder WithIndex<TIndexProvider>(this ContentPartOptionBuilder builder)
        where TIndexProvider : class, IIndexProvider
    {
        builder.Services.AddSingleton<IIndexProvider, TIndexProvider>();
        return builder;
    }

    /// <summary>
    /// Adds an <see cref="IDataMigration"/> to the service collection while maintaining the call chain for the content
    /// part builder.
    /// </summary>
    public static ContentPartOptionBuilder WithMigration<TMigration>(this ContentPartOptionBuilder builder)
        where TMigration : IDataMigration
    {
        builder.Services.AddScoped(typeof(IDataMigration), typeof(TMigration));
        return builder;
    }

    /// <summary>
    /// Adds an <see cref="IDataMigration"/> and an <see cref="IIndexProvider"/> to the service collection while
    /// maintaining the call chain for the content part builder.
    /// </summary>
    public static ContentPartOptionBuilder WithMigration<TMigration, TIndexProvider>(
        this ContentPartOptionBuilder builder)
        where TMigration : IDataMigration
        where TIndexProvider : class, IIndexProvider =>
        builder.WithMigration<TMigration>().WithIndex<TIndexProvider>();
}
