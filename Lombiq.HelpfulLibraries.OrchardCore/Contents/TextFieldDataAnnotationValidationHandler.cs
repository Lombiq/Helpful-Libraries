using Microsoft.Extensions.Localization;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

// Only used for the IStringLocalizer below.
public class TextFieldDataAnnotationValidationHandler;

/// <summary>
/// Adds automatic validation for <see cref="TextField"/> fields by referencing a <typeparamref name="TPart"/>'s data
/// annotations.
/// </summary>
/// <remarks>
/// <para>The supported types are listed below.</para>
/// <list type="bullet">
///     <item>
///         <description><see cref="MaxLengthAttribute"/></description>
///     </item>
///     <item>
///         <description><see cref="StringLengthAttribute"/></description>
///     </item>
///     <item>
///         <description><see cref="MinLengthAttribute"/></description>
///     </item>
///     <item>
///         <description><see cref="RequiredAttribute"/></description>
///     </item>
/// </list>
/// </remarks>
public class TextFieldDataAnnotationValidationHandler<TPart> : ContentFieldHandler<TextField>
    where TPart : ContentPart
{
    private readonly IUpdateModelAccessor _updateModelAccessor;
    private readonly IStringLocalizer<TextFieldDataAnnotationValidationHandler> T;
    public TextFieldDataAnnotationValidationHandler(
        IUpdateModelAccessor updateModelAccessor,
        IStringLocalizer<TextFieldDataAnnotationValidationHandler> stringLocalizer)
    {
        _updateModelAccessor = updateModelAccessor;
        T = stringLocalizer;
    }

    public override Task UpdatingAsync(UpdateContentFieldContext context, TextField field)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(field);

        Validate(
            context.ContentPartFieldDefinition,
            field,
            (memberName, message) => _updateModelAccessor.ModelUpdater.ModelState.AddModelError(memberName, message));

        return Task.CompletedTask;
    }

    public override Task ValidatingAsync(ValidateContentFieldContext context, TextField field)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(field);

        Validate(context.ContentPartFieldDefinition, field, (memberName, message) => context.Fail(message, memberName));

        return Task.CompletedTask;
    }

    private void Validate(
        ContentPartFieldDefinition definition,
        TextField field,
        Action<string, LocalizedString> onError)
    {
        var property = typeof(TPart)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .FirstOrDefault(property => property.Name == definition.Name);

        if (property == null) return;

        var attributes = property.GetCustomAttributes(inherit: false);

        foreach (var attribute in attributes)
        {
            ValidateAttribute(definition, field, attribute, onError);
        }
    }

    private void ValidateAttribute(
        ContentPartFieldDefinition definition,
        TextField field,
        object attribute,
        Action<string, LocalizedString> onError)
    {
        var memberName = $"{definition.PartDefinition.Name}.{definition.Name}.{nameof(TextField.Text)}";
        switch (attribute)
        {
            case MaxLengthAttribute { Length: { } maxLength }:
                if ((field.Text?.Length ?? 0) > maxLength)
                {
                    onError(memberName, T["The field {0} must be a string with a maximum length of {1}.", definition.Name, maxLength]);
                }

                break;
            case StringLengthAttribute { MaximumLength: { } maxLength }:
                if ((field.Text?.Length ?? 0) > maxLength)
                {
                    onError(memberName, T["The field {0} must be a string with a maximum length of {1}.", definition.Name, maxLength]);
                }

                break;
            case MinLengthAttribute { Length: { } minLength }:
                if ((field.Text?.Length ?? 0) < minLength)
                {
                    onError(memberName, T["The field {0} must be a string with a minimum length of {1}.", definition.Name, minLength]);
                }

                break;
            case RequiredAttribute:
                if (string.IsNullOrWhiteSpace(field.Text))
                {
                    onError(memberName, T["The {0} field is required.", definition.Name]);
                }

                break;
            default:
                // Nothing to do here.
                break;
        }
    }
}
