# Lombiq Helpful Libraries - Command Line Libraries



## About

This project helps with executing command line calls. Some of it uses [CliWrap](https://github.com/Tyrrrz/CliWrap).

For general details about and on using the Helpful Libraries see the [root Readme](../Readme.md).


## Documentation

- [`CliProgram`](CliProgram.cs): Makes it easier to call CliWrap with the same command.

### Helpers

- [`CliWrapHelper`](Helpers/CliWrapHelper.cs): Specialized tasks using CliWrap, such as `WhichAsync` that helps locate the full path of a program that's inside the user's PATH environment variable. 
- [`OperatingSystemHelper`](Helpers/OperatingSystemHelper.cs): Gets OS-specific information like the file extension of executable files.

### Extensions

- [`CommandExtensions`](Extensions/CommandExtensions.cs): Contains extension methods for CliWrap's `Command` type. For example: `ExecuteDotNetApplicationAsync` for launching .NET server apps that say "Application started. Press Ctrl+C to shut down.".
