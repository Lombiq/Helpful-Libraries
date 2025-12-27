using Lombiq.HelpfulLibraries.OrchardCore.Mvc;
using Lombiq.HelpfulLibraries.Samples.Controllers;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using Shouldly;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Samples.Tests.UI.Extensions;

public static class TestCaseUITestContextExtensions
{
    public static async Task TestLinqToDbSamplesAsync(this UITestContext context)
    {
        await context.EnableFeatureDirectlyAsync("Lombiq.HelpfulLibraries.Samples");

        using var client = context.CreateHttpClient();

        await AssertSimpleQueryAsync(context, client);
        await AssertJoinQueryAsync(context, client);
        await AssertCrudAsync(context, client);
    }

    private static async Task AssertSimpleQueryAsync(UITestContext context, HttpClient client)
    {
        var simpleQueryUrl = context.GetAbsoluteUrlOfAction<LinqToDbSamplesController>(controller => controller.SimpleQuery());
        var actualContent = await client.GetStringAsync(simpleQueryUrl, context.Configuration.TestCancellationToken);

        actualContent.ShouldMatchApproved(
            options => options
                .WithScrubber(ScrubContentItemIds)
                .WithFileExtension("json"),
            $"{simpleQueryUrl} results differ from expected content.");
    }

    private static async Task AssertJoinQueryAsync(UITestContext context, HttpClient client)
    {
        var joinQueryUrl = context.GetAbsoluteUrlOfAction<LinqToDbSamplesController>(controller => controller.JoinQuery());
        var joinQueryOutput = await client.GetStringAsync(joinQueryUrl, context.Configuration.TestCancellationToken);

        joinQueryOutput.ShouldBe("[\"Man must explore, and this is exploration at its greatest\"]");
    }

    private static async Task AssertCrudAsync(UITestContext context, HttpClient client)
    {
        var crudUrl = context.GetAbsoluteUrlOfAction<LinqToDbSamplesController>(controller => controller.Crud());
        var crudResult = await client.GetStringAsync(crudUrl, context.Configuration.TestCancellationToken);

        crudResult.ShouldBe("Inserted: 1, modified: 1, deleted: 1.");
    }

    private static string ScrubContentItemIds(string json)
    {
        var jsonArray = JsonNode.Parse(json) as JsonArray;

        jsonArray.ShouldNotBeNull("SimpleQuery output is not a JSON array.");

        foreach (var item in jsonArray)
        {
            item["contentItemId"] = string.Empty;
            item["containedContentItemId"] = string.Empty;
        }

        return jsonArray.ToJsonString();
    }
}
