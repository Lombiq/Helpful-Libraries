// Original source code: https://abdus.dev/posts/aspnetcore-model-binding-json-query-params/.
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.AspNetCore.Mvc;

public class JsonModelBinder(ILogger<JsonModelBinder> logger, IObjectModelValidator validator) : IModelBinder
{
    private static readonly JsonSerializerOptions SerializeOptions = new(JsonSerializerDefaults.Web);

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var value = bindingContext.ValueProvider.GetValue(bindingContext.FieldName).FirstValue;

        try
        {
            var parsed = value is null ? null : JsonSerializer.Deserialize(
                value,
                bindingContext.ModelType,
                SerializeOptions);

            if (parsed is null)
            {
                return Task.CompletedTask;
            }

            validator.Validate(
                bindingContext.ActionContext,
                validationState: bindingContext.ValidationState,
                prefix: string.Empty,
                model: parsed);

            bindingContext.Result = ModelBindingResult.Success(parsed);
        }
        catch (JsonException jsonException)
        {
            logger.LogError(jsonException, "Failed to bind parameter '{FieldName}'", bindingContext.FieldName);
            bindingContext.ActionContext.ModelState.TryAddModelError(
                key: jsonException.Path,
                exception: jsonException,
                bindingContext.ModelMetadata);
        }
        catch (Exception exception) when (exception is FormatException or OverflowException)
        {
            logger.LogError(exception, "Failed to bind parameter '{FieldName}'", bindingContext.FieldName);
            bindingContext.ActionContext.ModelState.TryAddModelError(
                string.Empty,
                exception,
                bindingContext.ModelMetadata);
        }

        return Task.CompletedTask;
    }
}
