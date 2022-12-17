using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lombiq.HelpfulLibraries.AspNetCore.Mvc;

public sealed class FromJsonQueryStringAttribute : ModelBinderAttribute
{
    public FromJsonQueryStringAttribute()
    {
        BinderType = typeof(JsonModelBinder);
        BindingSource = BindingSource.Query;
    }
}
