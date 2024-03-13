using Lombiq.HelpfulLibraries.Tests.Models;
using Shouldly;
using System;
using Xunit;

namespace Lombiq.HelpfulLibraries.Tests.UnitTests.SourceGenerators;

public class ConstantFromJsonTests
{
    [Fact]
    public void TestGeneratedConstants()
    {
        Examples.GulpVersion.ShouldBe("3.9.0");
        Examples.GulpUglifyVersion.ShouldBe("1.4.1");
        new Examples()
            .ReturnVersions()
            .Split(["\n", "\r"], StringSplitOptions.RemoveEmptyEntries)
            .ShouldBe(["Gulp version: 3.9.0", "Gulp-uglify version: 1.4.1"]);
    }
}
