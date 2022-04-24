using OrchardCore.ContentManagement.Metadata.Settings;
using System;
using System.Linq.Expressions;

namespace OrchardCore.ContentManagement.Metadata.Builders;

public static class ContentPartDefinitionBuilderExtensions
{
    /// <summary>
    /// Wraps an existing <see cref="ContentPartDefinitionBuilder"/> into a wrapped type.
    /// </summary>
    public static ContentPartDefinitionBuilder<TPart> AsPart<TPart>(this ContentPartDefinitionBuilder builder)
        where TPart : ContentPart =>
        new(builder);
}

public class ContentPartDefinitionBuilder<TPart>
    where TPart : ContentPart
{
    public ContentPartDefinitionBuilder Builder { get; private set; }

    public ContentPartDefinitionBuilder(ContentPartDefinitionBuilder builder) => Builder = builder;

    public ContentPartDefinitionBuilder<TPart> Configure(Action<ContentPartDefinitionBuilder> configureAction)
    {
        configureAction(Builder);
        return this;
    }

    /// <summary>
    /// Creates a field by getting the technical name and field type using a lambda expression rather than relying on
    /// developer provided strings. This improves the coupling between the part and the field. Also sets the display
    /// name to the field's technical name by default, but if a more natural name is needed it can be overwritten in the
    /// <paramref name="configuration"/>.
    /// </summary>
    /// <param name="fieldPropertySelector">The expression which gets the part and returns its field.</param>
    /// <param name="configuration">Any further configuration, can override the automatic settings.</param>
    /// <typeparam name="TField">The type of the new field to be attached to the content part.</typeparam>
    public ContentPartDefinitionBuilder<TPart> WithField<TField>(
        Expression<Func<TPart, TField>> fieldPropertySelector,
        Action<ContentPartFieldDefinitionBuilder> configuration = null)
    {
        var property = ((MemberExpression)fieldPropertySelector.Body).Member;
        var name = property.Name;

        Builder = Builder.WithField(
            name,
            field =>
            {
                field = field.WithDisplayName(name).OfType(typeof(TField).Name);
                configuration?.Invoke(field);
            });

        return this;
    }
}
