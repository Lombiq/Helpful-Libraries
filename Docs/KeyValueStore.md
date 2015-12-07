# Key Value Store Documentation



Key Value Store is a simple persistent key-value store. Just inject IKeyValueStore, the interface is very straightforward.

**When to use Key Value Store?** When you have to store items that should be accessible only by a single key (even more if there will be only a low number of items ever) so creating a separate table for it would be an overkill.

**When not to use Key Value Store?** When you want to query items on multiple ways, not just by a single key.


## Examples

	// _keyValueStore is an injected IKeyValueStore instance.
	_keyValueStore.Set("key", myObject);
	myObject = _keyValueStore.Get<MyType>("key");
	var itemExists = _keyValueStore.Exists("key");
	_keyValueStore.Remove("key");

There are some other handy extension methods too but the basic interface is this simple.