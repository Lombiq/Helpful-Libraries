# Lombiq Helpful Libraries - Orchard Core Libraries - Middlewares


Contains Orchard Core specific ASP.NET Core middlewares and middleware-related services. 

## `AutoroutePart` Middleware

Catches any URLs starting with _/Contents/ContentItems/_ and redirects to the path in their `AutoroutePart` if one exists. 
