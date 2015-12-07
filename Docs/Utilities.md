# Utilities Documentation



## Freezable

This is a lightweight implementation of the Freezable class. A freezable class is initially freely modifiable, but can be made read-only with freezing it.
The IFreezable interface describes a freezable class, the FreezableBase abstract class is a good base for freezable classes.

### Example

	public abstract class MyFreezable : FreezableBase
	{
	    private string _data;
	    public string Data
	    {
	        get { return _data; }
	        set
	        {
	            // This method will throw ReadonlyException if the object's Freeze() method was called.
	            ThrowIfFrozen();
	            _data= value;
	        }
	    }
	}


## Extensions

Various extensions methods that enhance built-in functionality. Don't forget to add the using `Piedone.HelpfulLibraries.Utilities;` declaration to your code.

### Examples

	// CacheServiceMonitor
	// _cacheService is an injected Orchard.Caching ICacheService.
	var value = _cacheService.Get<MyClass>("key", () => 
	{
	    // If the event is triggered the entry for the cache key will be removed. It will work otherwise too but it's better to call Monitor() only when the cache entry is created newly.
	    _cacheService.Monitor("event key", "cache key");
	
	    // ...
	});
	
	// Removes all cache entries that were subscribed to the specified event.
	_cacheService.Trigger("event key");
	// You can use the above to invalidate multiple cache entries at once: when they Monitor the same event key, triggering the event will remove all of the cache entries.
	
	// HqlQueryExtensions
	// From a Projector IFilterProvider
	public void ApplyFilter(FilterContext context)
	{
	    IEnumerable<int> ids = ...
	    context.Query.WhereIdIn(ids); // Applies the IN() in partitions of given size so the maximal number of IN() arguments isn't an issue.
	}
	// HqlExpressionFactoryExtensions has an extension to use such partitioned clauses in a generic way.
	
	// UriExtensions
	var urlWithoutSchema = new Uri("http://orchardproject.net").ToStringWithoutScheme();
	
	// WebViewPageExtensions
	if (this.WasNotDisplayed("myKey"))
	{
	    // Display something that should only be displayed once, no matter how many times the view is rendered
	}
	
	// Usable in a views, similar to the built-in Capture() method.
	@using (this.CaptureOnce())
	{
	    // This will be included in the resulting html markup only once, no matter how many times the block is run.
	}

See [this blogpost](https://english.orchardproject.hu/blog/making-sure-your-inline-script-is-only-incuded-once-when-multiple-content-items-are-listed) for a sample usage of CaptureOnce().


## MappingsManager

Service for managing ORM mappings, particularly for clearing NHibernate mappings by deleting mappings.bin. Can be useful if you want to use new mappings immediately their update (e.g. inside a migration).

	_mappingsManager.Clear(); // _mappingsManager is an injected IMappingsManager instance


## MimeAssistant

A simple helper for determining the MIME type of a file, depending on its extension. Its code is taken from a [SO post](http://stackoverflow.com/a/7161265/220230).

	var mime = MimeAssistant.GetMimeType("test.jpg"); // image/jpeg

## OrchardVersion

The following code returns the version of the currently executing Orchard application.

	var version = OrchardVersion.Current();
 
## UriHelper

Combines uri segments with forward slashes (much like Path.Combine() for local paths).

	var uri = UriHelper.Combine("sub", "folder"); // Returns sub/folders


## LocalizerDependency

ILocalizerDependency can be used to inject a Localizer through ctor injection (instead of the usual property injection) or to enable explicit Resolve<T>()-ing of it.