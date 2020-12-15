# User Libraries Documentation



## `ICachingUserServer` service

Retrieves `User`s from a transient per-request cache or sets them if they are not set yet. This is an abstraction over the `IUserService` using their methods to retrieve the `User` from the database but caches them after the first time. Can improve performance if the User is retrieved multiple times per request.

Please see the inline documentation of each methods to learn more.
