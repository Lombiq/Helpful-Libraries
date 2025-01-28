using Lombiq.HelpfulLibraries.Common.Utilities;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Lombiq.HelpfulLibraries.Tests.UnitTests.Utilities;

public class VersionTreeTests
{
    private static readonly JsonSerializerOptions _indentedJsonOption = new() { WriteIndented = true };

    private static readonly List<Version> _versions =
        "1.0 1.2 1.2.1 1.2.2 1.2.3 1.3 1.4 1.4.1 1.4.1.1 1.4.1.2 1.4.1.3 1.4.2.1 1.4.2.2 1.4.2.4"
            .Split()
            .Select(Version.Parse)
            .ToList();

    [Fact]
    public void VersionTreeShouldHaveExpectedStructure()
    {
        var tree = VersionTree.Create(_versions);
        tree.Versions.ShouldBe(_versions);

        ShouldMatchJson(tree, File.ReadAllText("VersionTreeTests.FullStructure.json"));
    }

    [Fact]
    public void IndexingByNumberAndVersionShouldWork()
    {
        var tree = VersionTree.Create(_versions);
        var expectedRevision = new Version(1, 4, 1, 1);

        tree[1]![4]![1]![1]!.Versions.ShouldBe([expectedRevision]);
        tree[expectedRevision]!.Versions.ShouldBe([expectedRevision]);
    }

    [Fact]
    public void VersionSubtreeShouldHaveExpectedStructure()
    {
        var tree = VersionTree.Create(_versions);
        var expectedSubtree = File.ReadAllText("VersionTreeTests.Subtree.json");

        // Verify full subtree structure.
        ShouldMatchJson(tree[1]![4], expectedSubtree);

        // Verify that indexing -1 results in the same.
        ShouldMatchJson(tree[1]![4]![-1]![-1], expectedSubtree);
        ShouldMatchJson(tree[new Version(1, 4)], expectedSubtree);
    }

    private static void ShouldMatchJson(VersionTree tree, string expectedJson)
    {
        var actualJson = JsonSerializer.Serialize(tree, _indentedJsonOption);
        (string.Join(string.Empty, actualJson.Split()) == string.Join(string.Empty, expectedJson.Split()))
            .ShouldBeTrue($"Actual JSON:\n{actualJson}\nExpected JSON: {expectedJson}\n(whitespace does not matter)");
    }
}
