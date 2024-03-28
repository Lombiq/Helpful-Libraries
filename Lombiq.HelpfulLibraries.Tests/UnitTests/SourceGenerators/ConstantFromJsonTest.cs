using Lombiq.HelpfulLibraries.Tests.Models;
using Shouldly;
using System;
using Xunit;

namespace Lombiq.HelpfulLibraries.Tests.UnitTests.SourceGenerators;

public class ConstantFromJsonTest
{
    [Fact]
    public void TestGeneratedConstants()
    {
        ConstantFromJsonSample.GulpVersion.ShouldBe("3.9.0");
        ConstantFromJsonSample.GulpUglifyVersion.ShouldBe("1.4.1");
        new ConstantFromJsonSample()
            .ReturnVersions()
            .Split(["\n", "\r"], StringSplitOptions.RemoveEmptyEntries)
            .ShouldBe(["Gulp version: 3.9.0", "Gulp-uglify version: 1.4.1"]);
    }
}
