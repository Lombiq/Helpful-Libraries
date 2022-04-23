# Lombiq Helpful Libraries - ASP.NET Core Libraries - Middlewares


Contains ASP.NET Core middlewares and middleware-related services. 

## Deferred Task

Implement the `IDeferredTask` interface if you want to create a service that executes near the end of the middleware pipeline where all filters and most other middlewares have already finished. You need to register it as a _Scoped_ service.

Also use the `app.UseDeferredTasks()` extension method to enable the necessary `DeferredTaskMiddleware`. You only have to do that once.

Note that deferred tasks won't execute on calls that aren't part of the middleware pipeline, such as deployment and setup tasks in Orchard Core. You can use the `IDeferredTask.IsScheduled` property to check if you can expect the deferred task to execute in the current scope. 
