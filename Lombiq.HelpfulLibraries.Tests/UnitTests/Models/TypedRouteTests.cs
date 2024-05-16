using Lombiq.HelpfulLibraries.OrchardCore.Mvc;
using Lombiq.HelpfulLibraries.Tests.Controllers;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Admin;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace Lombiq.HelpfulLibraries.Tests.UnitTests.Models;

public class TypedRouteTests
{
    public static readonly TheoryData<TypedRouteShouldWorkCorrectlyInput> TypedRouteShouldWorkCorrectlyInputs =
        new(GenerateTypedRouteShouldWorkCorrectlyInputs());

    [Theory]
    // TypedRouteShouldWorkCorrectlyInput would need to implement IXunitSerializable but it works like this anyway.
#pragma warning disable xUnit1045 // Avoid using TheoryData type arguments that might not be serializable
    [MemberData(nameof(TypedRouteShouldWorkCorrectlyInputs))]
#pragma warning restore xUnit1045 // Avoid using TheoryData type arguments that might not be serializable
    public void TypedRouteShouldWorkCorrectly(TypedRouteShouldWorkCorrectlyInput input)
    {
        using var serviceProvider = CreateServiceProvider();

        var route = TypedRoute.CreateFromExpression(
            input.ActionExpression,
            input.Additional,
            serviceProvider: serviceProvider);
        route.ToString(input.TenantName).ShouldBe(input.Expected);
    }

    [Fact]
    public void CustomizedAdminPrefixShouldBeUsed()
    {
        const string expected = "/CustomAdmin/Lombiq.HelpfulLibraries.Tests/RouteTest/Baz";

        using var serviceProvider = CreateServiceProvider(services => services
                .Configure<AdminOptions>(options => options.AdminUrlPrefix = " /CustomAdmin /"));

        var route = TypedRoute.CreateFromExpression(
            AsExpression(controller => controller.Baz()),
            serviceProvider: serviceProvider);
        route.ToString(tenantName: string.Empty).ShouldBe(expected);
    }

    [Theory]
    [InlineData("/content/1/", null, null)]
    [InlineData("/content/1/?anotherValue=foo", null, "foo")]
    [InlineData("/content/1/this", "this", null)]
    [InlineData("/content/1/that?anotherValue=etc", "that", "etc")]
    public void OptionalRouteSubstitutionShouldWork(string expected, string optionalArgument, string anotherValue)
    {
        using var serviceProvider = CreateServiceProvider();

        var route = TypedRoute.CreateFromExpression(
            AsExpression(controller => controller.RouteSubstitutionOptional(1, optionalArgument, anotherValue)),
            serviceProvider: serviceProvider);
        route.ToString().ShouldBe(expected);
    }

    private static ServiceProvider CreateServiceProvider(Action<ServiceCollection> configure = null)
    {
        var services = new ServiceCollection();

        const string feature = "Lombiq.HelpfulLibraries.Tests";
        var typeFeatureProvider = new TypeFeatureProvider();
        typeFeatureProvider.TryAdd(typeof(RouteTestController), new FeatureInfo(feature, new ExtensionInfo(feature)));
        services.AddSingleton<ITypeFeatureProvider>(typeFeatureProvider);

        services.AddMemoryCache();

        configure?.Invoke(services);

        return services.BuildServiceProvider();
    }

    private static Expression<Action<RouteTestController>> AsExpression(
        Expression<Action<RouteTestController>> expression) =>
        expression;

    public static IEnumerable<TypedRouteShouldWorkCorrectlyInput> GenerateTypedRouteShouldWorkCorrectlyInputs()
    {
        var noMoreArguments = Array.Empty<(string Name, object Value)>();
        var noTenant = string.Empty;
        var someTenant = "SomeTenant";

        var tests = new List<TypedRouteShouldWorkCorrectlyInput>
        {
            new(
                "/Lombiq.HelpfulLibraries.Tests/RouteTest/Foo",
                AsExpression(controller => controller.Foo()),
                noMoreArguments,
                noTenant),
            new(
                "/Lombiq.HelpfulLibraries.Tests/RouteTest/Bar",
                AsExpression(controller => controller.Bar()),
                noMoreArguments,
                noTenant),
            new(
                "/Admin/Lombiq.HelpfulLibraries.Tests/RouteTest/Baz",
                AsExpression(controller => controller.Baz()),
                noMoreArguments,
                noTenant),
            new(
                "/SomeTenant/Lombiq.HelpfulLibraries.Tests/RouteTest/Foo",
                AsExpression(controller => controller.Foo()),
                noMoreArguments,
                someTenant),
            new(
                "/SomeTenant/Lombiq.HelpfulLibraries.Tests/RouteTest/Bar",
                AsExpression(controller => controller.Bar()),
                noMoreArguments,
                someTenant),
            new(
                "/SomeTenant/Admin/Lombiq.HelpfulLibraries.Tests/RouteTest/Baz",
                AsExpression(controller => controller.Baz()),
                noMoreArguments,
                someTenant),
            new(
                "/I/Am/Routed",
                AsExpression(controller => controller.Route()),
                noMoreArguments,
                noTenant),
            new(
                "/I/Am/Routed/Admin",
                AsExpression(controller => controller.AdminRoute()),
                noMoreArguments,
                noTenant),
            new(
                "/I/Am/Routed?wat=is+this",
                AsExpression(controller => controller.Route()),
                [("wat", "is this")],
                noTenant),
            new(
                "/content/10",
                AsExpression(controller => controller.RouteSubstitution(10)),
                noMoreArguments,
                noTenant),
        };

        // Here we test multiple arguments and also overlapping variable names to ensure it doesn't generate clashing
        // cache keys. If that were the case, the second of the two usages would fail.
        void AddArgumentsTest(int addDays, string expect)
        {
            var date = new DateTime(1997, 8, 29, 2, 14, 0, DateTimeKind.Utc).AddDays(addDays);

            tests.Add(new(
                expect,
                AsExpression(controller => controller.Arguments(9001, 2.71, date, "done")),
                noMoreArguments,
                noTenant));
        }

        AddArgumentsTest(
            0,
            "/Lombiq.HelpfulLibraries.Tests/RouteTest/Arguments?number=9001&fraction=2.71&dateTime=1997-08-29T02%3A14%3A00&text=done");
        AddArgumentsTest(
            1,
            "/Lombiq.HelpfulLibraries.Tests/RouteTest/Arguments?number=9001&fraction=2.71&dateTime=1997-08-30T02%3A14%3A00&text=done");

        return tests;
    }

    public class TypedRouteShouldWorkCorrectlyInput(
        string expected,
        Expression<Action<RouteTestController>> actionExpression,
        IEnumerable<(string Key, object Value)> additional,
        string tenantName)
    {
        public string Expected { get; set; } = expected;
        public Expression<Action<RouteTestController>> ActionExpression { get; set; } = actionExpression;
        public IEnumerable<(string Key, object Value)> Additional { get; set; } = additional;
        public string TenantName { get; set; } = tenantName;
    }
}
