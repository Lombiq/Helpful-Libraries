using Lombiq.HelpfulLibraries.OrchardCore.Mvc;
using Lombiq.HelpfulLibraries.Samples.Controllers;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
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
        var simpleQueryOutput = await client.GetStringAsync(simpleQueryUrl, context.Configuration.TestCancellationToken);
        var simpleQueryParsed = JsonSerializer.Deserialize<IList<Dictionary<string, object>>>(simpleQueryOutput);

        // Cleanup received data.
        simpleQueryParsed = simpleQueryParsed
            .Select(item => item
                // The "id" and "documentId" properties depend on the record's write order in the database. They are not
                // guaranteed to be consistent on different setups, and they are not relevant for this query.
                .Where(pair => pair.Key is not "id" and not "documentId")
                // The order of these properties is not guaranteed either, so sorting them here makes it more reliable.
                .OrderBy(pair => pair.Key)
                .ToDictionary(pair => pair.Key, pair => pair.Value))
            .ToList();

        // The results are re-serialized into JSON using standard options, so they can be compared in a consistent way.
        var reserialized = JsonSerializer.Serialize(simpleQueryParsed, JOptions.Default);
        reserialized.ShouldMatchApproved(
            options => options
                .WithScrubber(ScrubContentItemIds)
                .WithFileExtension("json"),
            $"{simpleQueryUrl} results differ from expected content.");
    }

    private static async Task AssertJoinQueryAsync(UITestContext context, HttpClient client)
    {
        var joinQueryUrl = context.GetAbsoluteUrlOfAction<LinqToDbSamplesController>(controller => controller.JoinQuery());
        var joinQueryOutput = await client.GetStringAsync(joinQueryUrl, context.Configuration.TestCancellationToken);

        JsonSerializer
            .Deserialize<string[]>(joinQueryOutput)
            .ShouldBe(["Man must explore, and this is exploration at its greatest"]);
    }

    private static async Task AssertCrudAsync(UITestContext context, HttpClient client)
    {
        var crudUrl = context.GetAbsoluteUrlOfAction<LinqToDbSamplesController>(controller => controller.Crud());
        var crudResult = await client.GetStringAsync(crudUrl, context.Configuration.TestCancellationToken);

        crudResult.ShouldBe("Inserted: 1, modified: 1, deleted: 1.");
    }

    private static string ScrubContentItemIds(string json)
    {
        var jsonArray = JsonNode
            .Parse(json)
            .ShouldBeOfType<JsonArray>($"SimpleQuery output is not a JSON array. JSON:{Environment.NewLine}{json}");

        foreach (var item in jsonArray)
        {
            item["contentItemId"] = string.Empty;
            item["containedContentItemId"] = string.Empty;
        }

        return jsonArray.ToJsonString();
    }
}
