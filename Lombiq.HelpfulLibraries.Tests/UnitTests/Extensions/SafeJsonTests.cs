using Lombiq.Tests.Integration.Controllers;
using Lombiq.Tests.Integration.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq.AutoMock;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Lombiq.HelpfulLibraries.Tests.UnitTests.Extensions;

public class SafeJsonTests
{
    private const string ExceptionText = "Intentional Exception";

    // The JsonResult in ASP.NET Core uses camelCase outputs so we have to replicate that.
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [Fact]
    public async Task ExceptionShouldBeLogged()
    {
        var loggerProvider = (await SetupAsync()).LoggerProvider;

        var logger = loggerProvider
            .CreateLogger(typeof(AutoMockerController).FullName)
            .ShouldBeOfType<ListLogger>();
        logger.Logs.Count(log => log.LogLevel == LogLevel.Error).ShouldBe(2);
    }

    [Fact]
    public async Task JsonResultsShouldMatchExpectationsInDevelopment()
    {
        var (_, failure, failureAsync, success) = await SetupAsync();

        failure["error"].ShouldBe(ExceptionText);
        failure["data"].StartsWithOrdinal("System.InvalidOperationException:").ShouldBeTrue();

        failureAsync["error"].ShouldBe(ExceptionText);
        failureAsync["data"].StartsWithOrdinal("System.InvalidOperationException:").ShouldBeTrue();

        success["foo"].ShouldBe("bar");
        success["this"].ShouldBe(bool.TrueString);
        success["that"].ShouldBe("10");
    }

    [Fact]
    public async Task JsonResultsShouldMatchExpectationsInProduction()
    {
        var (_, failure, failureAsync, success) = await SetupAsync(controller =>
        {
            controller.UseProductionEnvironment();
            controller.SetRequestUri(new Uri("https://example.com/foo"));
        });

        failure["error"].ShouldBe("An error has occurred while trying to process your request.");
        failure.ShouldNotContainKey("data");

        failureAsync["error"].ShouldBe("An error has occurred while trying to process your request.");
        failure.ShouldNotContainKey("data");

        success["foo"].ShouldBe("bar");
        success["this"].ShouldBe(bool.TrueString);
        success["that"].ShouldBe("10");
    }

    private static async Task<TestResults> SetupAsync(Action<AutoMockerController> setupController = null)
    {
        var mocker = new AutoMocker();

        using var loggerProvider = mocker.MockLogging();
        mocker.MockStringLocalization();

        using var controller = new AutoMockerController(mocker);
        setupController?.Invoke(controller);

        var failure = await SafeJsonToDictionaryAsync(controller, () => throw new InvalidOperationException(ExceptionText));

        var failureAsync = await SafeJsonToDictionaryAsync(
            controller,
            async () =>
                {
                    await Task.Delay(1, CancellationToken.None);
                    throw new InvalidOperationException(ExceptionText);
                });

        var success = await SafeJsonToDictionaryAsync(
            controller,
            async () =>
                {
                    await Task.Delay(1, CancellationToken.None);
                    return new
                    {
                        Foo = "bar",
                        This = true,
                        That = 10,
                    };
                });

        return new(loggerProvider, failure, failureAsync, success);
    }

    private static async Task<Dictionary<string, string>> SafeJsonToDictionaryAsync(
        Controller controller,
        Func<Task<object>> factory)
    {
        var result = await controller.SafeJsonAsync(factory);

        return JsonSerializer.Deserialize<Dictionary<string, object>>(
            JsonSerializer.Serialize(result.Value, JsonSerializerOptions))!
            .ToDictionary(pair => pair.Key, pair => pair.Value.ToString());
    }

    private sealed record TestResults(
        ListLoggerProvider LoggerProvider,
        Dictionary<string, string> Failure,
        Dictionary<string, string> FailureAsync,
        Dictionary<string, string> Success);
}
