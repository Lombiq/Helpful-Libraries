# Tasks Libraries Documentation



With these libraries you can use Orchard functionality or other modules' services with System.Threading.Tasks.Task objects or with async callbacks (due to Orchard's context handling this is not trivial) as well as use site-wide (even across multiple servers) locking for task-like blocks and cache entry computations.


## Usage

### ILockingCacheManager

Through this interface you get ICacheManager-like functionality with the exception that if two concurrent requests (or threads) try to access the same cache entry and that entry doesn't exist yet then the slower request will wait for the other to compute the result; they won't compute the result simultaneously.
Usage (needs a constructor-injected ILockingCacheManager instance):

	var value = _lockingCacheManager.Get("cacheKey", ctx =>
	{
	    // This will be run if the lock could be acquired. That practically means, that's 
	    // the code that computes the cache entry's value; therefore also place 
	    // your Monitor calls here, just as with the standard CacheManager.
	
	    return computedValue
	}, () => 
	{
	    // This is a fallback that will be called if the lock couldn't be acquired before the specified timeout.
	}, 5000);
	// The last argument specifies the number of milliseconds the manager waits for the lock to be released before timing out and calling the fallback delegate.

The default implementation of ILockingCacheManager uses ICacheManager under the hood, so depending on its implementation the same rules apply too as with using ICacheManager directly.  
Note that the default implementation uses (zero-sized) files to implement locking functionality. This works in all well configured server environments, across all servers if there are more than one. However since writing a file takes some time in some extremely rare cases it can happen that one thread tries to acquire a lock that another already started, but not yet finished acquiring. This can result in exceptions from the storage provider. If you need faster locking on a single-server environment, just use standard static locking approaches.  
Lock files are saved to the Media/Tenant/HelpfulLibraries/Tasks/LockFiles folder. **If the application is terminated unexpectedly** (due to some catastrophic failure, like power outage; normal AppDomain shutdown is no problem) **it can cause lock files not to be deleted, causing locks not to be released. If such a failure happens, make sure to delete the folder before restarting Orchard.**

### IDetachedDelegateBuilder

By injecting IDetachedDelegateBuilder in your code's constructor you get access to the following:

- BuildAsyncEventHandler(): use this to build an action that can be run as an async callback
- BuildBackgroundAction(): this builds an action that can be executed in a background Task
- BuildBackgroundFunction(): this builds a function that can be executed in a background Task

Pay special attention to the "catchExceptions" argument with all methods; if true, the code you supply the builders will be surrounded by a try-catch, where the exception will be logged in the catch. Be extremely cautious to catch every possible exception if you set it to false as any uncaught and unobserved exception in a background thread causes the whole site to halt!
If in the code you want to execute in the background you don't use any Orchard services, it's just native ASP.NET MVC, you don't need IDetachedDelegateBuilder.

### IDistributedLockManager

This interface provided lower-level locking functionality where the lock state is shared across all the server nodes in a multi-server setup. For an example of the usage take a look at LockingCacheManager.Get().

### Enhanced Task Lease

The feature enables Task Lease to work properly in a multi-node setup if you also have a distributed Orchard.Caching implementation enabled.

### Jobs

Jobs is a separate feature, but under Tasks. It deals with managing pieces of tasks where a task should be processed only once.
Samples:

	// Classes needed for the sample
	public class Context
	{
	    public string Url { get; set; }
	}
	
	public interface IMyWorker : IAtomicWorker
	{
	}
	
	public class MyWorker : IMyWorker
	{
	    public void WorkOn(IJob job)
	    {
	        // Do something just as normally with jobs.
	    }
	}
	
	
	/*
	* JobManager samples
	*/
	// Inject an IJobManager instance to use these
	
	_jobManager.CreateJob("RssAggregation", new Context { Url = "http://example.com/feed.rss" }, 50);
	
	// This job, having higher priority, will be taken before the previous one
	_jobManager.CreateJob("RssAggregation", new Context { Url = "http://example2.com/feed.rss" }, 100);
	
	// Between the TakeJob() and Done() call no other process, even in a multi-server environment, can take the same job
	var job1 = _jobManager.TakeJob("RssAggregation");
	var context1 = job1.Context<Context>(); // http://example2.com/feed.rss
	// Processsing...
	_jobManager.Done(job1);
	
	var job2 = _jobManager.TakeJob("RssAggregation");
	var context2 = job1.Context<Context>(); // http://example.com/feed.rss
	// Processsing...
	_jobManager.GiveBack(job1); // We haven't completed the job for a reason, we give it back. It can thus be taken again.
	
	// Job1 and job2 will contain an available job. If you want to have only one job to be worked on however, use TakeOnlyJob().
	var onlyJob = _jobManager.TakeOnlyJob("RssAggregation");
	// Simliar processing than with other jobs.
	
	/*
	* AtomicJobQueue samples
	*/
	// Inject an IAtomicJobQueue instance to use these
	
	// This queues a job work: IMyWorker will be resolved and called with a job returned by TakeJob() (thus it can be null!).
	// This call happens in an own transaction scope, independent from requests or background tasks.
	// IMyWorker.WorkOn() (here: MyWorker.WorkOn()) will be called when the next request to the site ends.
	// You can queue the same worker for multiple industries, then you can check the industry of the job in WorkOn().
	_jobQueue.Queue<IMyWorker>("RssAggregation");