# User Libraries Documentation



## `ICachingUserManager`

Retrieves `User`s from a transient per-request cache or gets them from the store if not yet cached. This is an abstraction over the `UserManager<IUser>` using its methods to retrieve the `User` from the database but caches them after the first time. Can improve performance if the `User` is retrieved multiple times per request.

Please see the inline documentation of each method to learn more.
