using System;

namespace Lombiq.HelpfulLibraries.OrchardCore.Data;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ContentItemIdColumnAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class UnlimitedLengthAttribute : Attribute
{
}
