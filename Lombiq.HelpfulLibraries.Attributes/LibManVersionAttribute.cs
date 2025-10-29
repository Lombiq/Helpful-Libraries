using System;

namespace Lombiq.HelpfulLibraries.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class LibManVersionAttribute : Attribute
{
    public LibManVersionAttribute(string constantName, string packageName, string fileName = "libman.json")
    {
    }
}
