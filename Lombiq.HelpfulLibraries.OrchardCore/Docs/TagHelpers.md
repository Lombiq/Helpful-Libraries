# Lombiq Helpful Libraries - Orchard Core Libraries - Tag Helpers

Make sure to use `services.AddTagHelpers<T>()` in the `Startup` class and `@addTagHelper *, Lombiq.HelpfulLibraries.OrchardCore` in the __ViewImports.cshtml_ file.

## `EditorFieldSetTagHelper`

Eliminates significant boilerplate for editor fields in the admin dashboard. With this you can add `asp-for` to an empty `<fieldset>` element and skip the usual label, input, and validation elements that normally reside in them.

## `ShapeTagHelperBase`

A base class for tag helpers that return a shape. When you inherit from this type, the view-model returned by the `ShapeTagHelperBase.GetViewModelAsync()` abstract method will be available in your _.cshtml_ template in the `Model.ViewModel` property. So you can do the following:

```cshtml
var viewModel = (TypeOfYourViewModel)Model.ViewModel;
```
