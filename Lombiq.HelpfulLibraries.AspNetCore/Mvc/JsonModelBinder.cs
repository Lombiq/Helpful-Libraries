// Original source code: https://abdus.dev/posts/aspnetcore-model-binding-json-query-params/.
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.AspNetCore.Mvc;

public class JsonModelBinder : IModelBinder
{
    private static readonly JsonSerializerOptions DeSerializeOptions = new(JsonSerializerDefaults.Web);

    private readonly ILogger<JsonModelBinder> _logger;
    private readonly IObjectModelValidator _validator;

    public JsonModelBinder(ILogger<JsonModelBinder> logger, IObjectModelValidator validator)
    {
        _logger = logger;
        _validator = validator;
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var value = bindingContext.ValueProvider.GetValue(bindingContext.FieldName).FirstValue;

        try
        {
            var parsed = value is null ? null : JsonSerializer.Deserialize(
                value,
                bindingContext.ModelType,
                DeSerializeOptions);

            if (parsed is null)
            {
                return Task.CompletedTask;
            }

            _validator.Validate(
                bindingContext.ActionContext,
                validationState: bindingContext.ValidationState,
                prefix: string.Empty,
                model: parsed);

            bindingContext.Result = ModelBindingResult.Success(parsed);
        }
        catch (JsonException jsonException)
        {
            _logger.LogError(jsonException, "Failed to bind parameter '{FieldName}'", bindingContext.FieldName);
            bindingContext.ActionContext.ModelState.TryAddModelError(
                key: jsonException.Path,
                exception: jsonException,
                bindingContext.ModelMetadata);
        }
        catch (Exception exception) when (exception is FormatException or OverflowException)
        {
            _logger.LogError(exception, "Failed to bind parameter '{FieldName}'", bindingContext.FieldName);
            bindingContext.ActionContext.ModelState.TryAddModelError(
                string.Empty,
                exception,
                bindingContext.ModelMetadata);
        }

        return Task.CompletedTask;
    }
}
