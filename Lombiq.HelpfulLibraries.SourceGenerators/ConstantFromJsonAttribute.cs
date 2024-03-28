namespace Lombiq.HelpfulLibraries.SourceGenerators;

[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
public sealed class ConstantFromJsonAttribute : System.Attribute
{
    public ConstantFromJsonAttribute(string constantName, string fileName, string propertyName)
    {
    }
}
