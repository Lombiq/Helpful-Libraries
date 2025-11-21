using Lombiq.HelpfulLibraries.Attributes;
using Shouldly;
using System;
using Xunit;

namespace Lombiq.HelpfulLibraries.Tests.UnitTests.SourceGenerators;

[LibManVersions(fileName: "libman-sample.json")]
public partial class LibManResourceVersionGeneratorTest
{
    [Fact]
    public void TestGeneratedConstants()
    {
        new Version(LibManVersions.ChartJs).ShouldBe(new(4, 5, 1));
        new Version(LibManVersions.ChartjsPluginAnnotation).ShouldBe(new(3, 1, 0));
        new Version(LibManVersions.ChartjsPluginDatalabels).ShouldBe(new(2, 2, 0));
    }
}
