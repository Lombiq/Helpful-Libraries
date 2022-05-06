# Lombiq Helpful Libraries - Command Line Libraries



## About

This project helps with executing command line calls. Some of it uses [CliWrap](https://github.com/Tyrrrz/CliWrap).

For general details about and on using the Helpful Libraries see the [root Readme](../Readme.md).


## Documentation

- [`CliHelper`](Helpers/CliHelper.cs)
  - `WhichAsync`: Gets the absolute path of an executable that you can call directly form CLI (in `$PATH` on Unix-like systems or in `%PATH%` on Windows).
  - `StreamAsync`: Executes a program and then calls the provided handler on every command line event.

### Extensions

You can write custom SQL syntax extensions and functions as you can see it in *[Extensions/CustomSqlExtensions.cs](Extensions/CustomSqlExtensions.cs)*.

For more examples check out [this article](http://blog.linq2db.com/2016/06/how-to-teach-linq-to-db-convert-custom.html).
