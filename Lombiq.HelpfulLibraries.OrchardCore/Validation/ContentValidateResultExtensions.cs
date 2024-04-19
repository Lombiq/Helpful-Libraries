using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.ContentManagement.Handlers;
using System.Linq;

namespace Lombiq.HelpfulLibraries.OrchardCore.Validation;

public static class ContentValidateResultExtensions
{
    public static void AddValidationErrorsToModelState(this ContentValidateResult result, ModelStateDictionary modelState)
    {
        foreach (var error in result.Errors)
        {
            if (error.MemberNames.Any())
            {
                foreach (var memberName in error.MemberNames)
                {
                    modelState.AddModelError(memberName, error.ErrorMessage);
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, error.ErrorMessage);
            }
        }
    }
}
