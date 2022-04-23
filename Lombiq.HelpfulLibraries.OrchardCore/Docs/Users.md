# Lombiq Helpful Libraries - Orchard Core Libraries - Users for Orchard Core



## `AdminPermissionBase`

A provider that only has *Administrator* stereotype permissions. Reduces boilerplate.

To use it, override the `AdminPermissions` read-only abstract property with a collection of admin permissions. This base class implements the `GetPermissionsAsync` and `GetDefaultStereotypes` methods with the derived permission collection.

## Extensions

- `UserServiceExtensions`: Adds extensions for `IUserService`. (E.g. `GetOrchardUserAsync` retrieves the user by user name or throws an exception if none found.)

## `ICachingUserManager`

Retrieves `User`s from a non-persistent, per-request cache or gets them from the store if not yet cached. This is an abstraction over the `UserManager<IUser>` using its methods to retrieve the `User` from the database but caches them after the first time. Can improve performance if the `User` is retrieved multiple times per request.

## `RoleCommands`

Adds the `addPermissionToRole /RoleName:<rolename> /Permission:<permission>` command so you can easily assign permissions in the recipes. 
