using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace OrchardCore.DisplayManagement.ModelBinding;

public static class UpdateModelExtensions
{
    public static IEnumerable<ModelError> GetModelErrors(this IUpdateModel updateModel) =>
        updateModel
            .ModelState
            .Values
            .SelectMany(entry => entry.Errors)
            .Where(error => !string.IsNullOrWhiteSpace(error.ErrorMessage));

    public static IEnumerable<string> GetModelErrorMessages(this IUpdateModel updateModel) =>
        updateModel.GetModelErrors().Select(error => error.ErrorMessage);
}
