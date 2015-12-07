# Service Validation Libraries Documentation



## What the heck is this?

The classes in this feature try to aid the issue of returning validation errors from services.


## Examples

### ServiceValidationDictionary usage

For a real example check FacebookConnectService.Authorize() in the module Facebook Suite Connect.
You can also use the generic IServiceValidationDictionary to use any type as the validation dictionary key.

	public class MyService : IMyService, IValidatingService // IValidatingService contains the ValidationDictionary property
	{
	    public IServiceValidationDictionary ValidationDictionary { get; private set; }
	
	    public MyService(IServiceValidationDictionary validationDictionary)
	    {
	        ValidationDictionary = validationDictionary;
	    }
	
	    public SomeMethod()
	    {
	        ValidationDictionary.AddError("someError", T("This is the message."));
	    }
	}

### Validating a part in a service

If you want to manually validate a part in a service (although this is better done with an IUpdateModel instance in the driver method corresponding to the part), you can do this as following:

	var validationContext = new ValidationContext(part, null, null);
	var validationResults = new List<ValidationResult>();
	bool isValid = Validator.TryValidateObject(part, validationContext, validationResults, true);

### Notification example

Use this for example in controllers to display a notification of validation errors:

	if (!_service.ValidationDictionary.IsValid)
	{
	     foreach (var error in _service.ValidationDictionary.Errors)
	     {
	         _notifier.Add(NotifyType.Warning, error.Value);
	     }
	}

### Transcribe errors from service dictionary to an updater (IUpdateModel)

You can do this in a driver method:

	updater.TranscribeValidationDictionaryErrors(_service.ValidationDictionary);

This way notifications will automatically appear on the UI.
As this uses an extension method, don't forget to add this to the usings:

	using Piedone.HelpfulLibraries.ServiceValidation.Extensions;

### Transcribe errors from service dictionary to a ModelStateDictionary

You can do this in a controller action:

	ModelState.TranscribeValidationDictionaryErrors(_service.ValidationDictionary);

This way notifications will automatically appear on the UI.
As this uses an extension method, don't forget to add this to the usings:

	using Piedone.HelpfulLibraries.ServiceValidation.Extensions;