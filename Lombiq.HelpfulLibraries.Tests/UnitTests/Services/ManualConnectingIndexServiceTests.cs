using Lombiq.HelpfulLibraries.Tests.Models;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using YesSql;
using YesSql.Services;

namespace Lombiq.HelpfulLibraries.Tests.UnitTests.Services;

public class ManualConnectingIndexServiceTests : IClassFixture<ManualConnectingIndexServiceFixture>
{
    public const string NamePrefix = "test_";

    private readonly ManualConnectingIndexServiceFixture _fixture;

    public ManualConnectingIndexServiceTests(ManualConnectingIndexServiceFixture fixture) => _fixture = fixture;

    [Fact]
    public Task IndicesShouldHaveMatchingDocuments() => _fixture.SessionAsync(async session =>
    {
        var indices = (await session.QueryIndex<TestDocumentIndex>().ListAsync()).ToList();
        indices.ShouldNotBeEmpty();

        var documents = (await session.Query<TestDocument, TestDocumentIndex>().ListAsync())
            .ToDictionary(document => document.Name);
        foreach (var index in indices) documents.ShouldContainKey(NamePrefix + index.Number.ToTechnicalString());
    });

    [Fact]
    public Task AllIndexShouldRetrieveItsDocument() => _fixture.SessionAsync(async session =>
    {
        // In the example 3's index was intentionally skipped and 6's index was deleted after the fact.
        var numbers = Enumerable.Range(0, 10).Where(i => i is not 3 and not 6).ToList();
        var query = session.Query<TestDocument, TestDocumentIndex>(index => index.Number.IsIn(numbers));
        var list = await query.ListAsync();
        var documents = list.ToList();
        documents.Select(x => x.Name)
            .ShouldBe(_fixture.Documents.Where((_, index) => index is not 3 and not 6).Select(x => x.Name));
    });

    [Fact]
    public Task MissingOrDeletedIndexShouldNotRetreiveAnyDocument() => _fixture.SessionAsync(async session =>
    {
        var numbers = new[] { 3, 6 };
        var documents = (await session.Query<TestDocument, TestDocumentIndex>(x => x.Number.IsIn(numbers)).ListAsync()).ToList();
        documents.ShouldBeEmpty();
    });
}
