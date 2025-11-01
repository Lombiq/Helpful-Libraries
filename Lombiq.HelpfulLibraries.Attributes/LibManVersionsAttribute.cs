using System;

namespace Lombiq.HelpfulLibraries.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class LibManVersionsAttribute : Attribute
{
    public LibManVersionsAttribute(string fileName = "libman.json")
    {
    }
}
