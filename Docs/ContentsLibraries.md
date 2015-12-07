# Contents Libraries Documentation



## Dynamic pages

These extensions and interfaces make it easier to create [dynamic pages](http://english.orchardproject.hu/blog/the-orchard-dynamic-page-pattern). See the [Associativy project's](http://associativy.com/) [Administration](https://github.com/Lombiq/Associativy-Administration) and [Frontend Engines](https://github.com/Lombiq/Associativy-Frontend-Engines) modules for examples.


## PrefixedEditorManager

PrefixedEditorManager makes it possible to have multiple content item editors on the same page (even nesting them) by prefixing their fields with the content item IDs. PrefixedEditorManager is a contribution of [Onestop Internet, Inc](http://www.onestop.com/).

Sample:

	// Building shapes
	// _prefixedEditorManager is an injected IPrefixedEditorManager instance     
	
	foreach (var item in contentItems) {
	    // Inside the shape factory lambda we could use BuildEditor or any other shape building mechanism too
	    var editorShape = _prefixedEditorManager.BuildShape(item, (content => _contentManager.BuildDisplay(content)));
	    // Do something with the shape, e.g. add it to a list shape
	}
	
	// Updating items, same as ContentManager.UpdateEditor
	// We could get the list of items by using the same technique as Core.Contents AdminController bulk edit: include a form element with name "itemIds" for each item, then let the model binder give us all ids by having an IEnumerable<int> itemIds argument on the post back action.
	foreach (var item in items) {
	    _prefixedEditorManager.UpdateEditor(item, this);
	}


## Extensions

These add extension methods to IContent objects.  
Don't forget the `using Piedone.HelpfulLibraries.Contents;` declaration! Since all the methods are extension methods, they can't be found otherwise.

The below examples demonstrate how the extensions simplify the retrieval of fields contained by content parts.  
All methods have inline documentation.

	// Checking existence of field
	var hasField = item.HasField("PartName", "FieldName");
	hasField = item.HasField<FieldType>("PartName");
	
	// Field retrieval
	var field = item.AsField<FieldType>("PartName", "FieldName");
	field = item.AsField<FieldType>("PartName");
	
	// Storing a simple transient context in the content item object that's not persistent and lives as long as the object itself. You can use this to store arbitrary data on the content item object to pass around without having to add a dummy content part for it.
	var value = "value";
	contentItem.SetContext("key", value);
	var retrievedValue = contentItem.GetContext<string>("key");


## ContainedByFilter Projector filter

Moved to [Helpful Extensions](https://github.com/Lombiq/Helpful-Extensions).


## IdsInFilter Projector filter

Moved to [Helpful Extensions](https://github.com/Lombiq/Helpful-Extensions).