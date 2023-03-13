using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.Data.Migration;
using OrchardCore.DisplayManagement.Descriptors;
using System;
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

    /// <summary>
    /// Registers a driver that inherits from <see cref="SingleDisplayTypeContentPartDisplayDriver{TPart}"/>. If the
    /// driver has a single type argument that inherits from <see cref="ContentPart"/>, you can pass the generic type to
    /// <paramref name="driverType"/> and the part type will be pulled from the <paramref name="builder"/> instead.
    /// </summary>
    public static ContentPartOptionBuilder UseSingleDisplayTypeContentPartDisplayDriver(this ContentPartOptionBuilder builder, Type driverType)
    {
        if (driverType.GenericTypeArguments.Length == 1) driverType = driverType.GetGenericTypeDefinition();
        if (driverType.IsGenericType) driverType = driverType.MakeGenericType(builder.ContentPartType);

        var baseType = driverType.BaseType;
        while (baseType!.Name != typeof(SingleDisplayTypeContentPartDisplayDriver<>).Name) baseType = baseType.BaseType;

        var resolverType = typeof(SingleDisplayTypeContentPartDisplayDriver<>.FieldHiderPlacementInfoResolver<>)
            .MakeGenericType(builder.ContentPartType, driverType);

        builder.Services.AddScoped(typeof(IPlacementInfoResolver), resolverType);
        builder.Services.AddScoped(typeof(IShapePlacementProvider), resolverType);

        return builder.ForDisplayMode(driverType);
    }

    /// <summary>
    /// Registers <see cref="DetailOnlyContentPartDisplayDriver{TPart}"/> as the part's display driver.
    /// </summary>
    public static ContentPartOptionBuilder UseDetailOnlyDriver(this ContentPartOptionBuilder builder) =>
        builder.UseSingleDisplayTypeContentPartDisplayDriver(typeof(DetailOnlyContentPartDisplayDriver<>));
}
