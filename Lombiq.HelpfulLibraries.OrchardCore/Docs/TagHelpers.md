# Lombiq Helpful Libraries - Orchard Core Libraries - Tag Helpers

Make sure to use `services.AddTagHelpers<T>()` in the `Startup` class and `@addTagHelper *, Lombiq.HelpfulLibraries.OrchardCore` in the __ViewImports.cshtml_ file.

- `EditorFieldSetTagHelper`: Eliminates significant boilerplate for editor fields in the admin dashboard. With this you can add `asp-for` to an empty `<fieldset>` element and skip the usual label, input, and validation elements that normally reside in them. 
