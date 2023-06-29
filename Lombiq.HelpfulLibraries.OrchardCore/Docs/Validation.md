# Lombiq Helpful Libraries - Orchard Core Libraries - Validation for Orchard Core

## `IEmailAndPasswordValidator`

A service for returning validation errors for email of password. Use its `ValidateEmailAsync` method to get email format validation errors, and its `ValidatePasswordAsync` to get validation errors from the registered `IPasswordValidator<IUser>` instances.
