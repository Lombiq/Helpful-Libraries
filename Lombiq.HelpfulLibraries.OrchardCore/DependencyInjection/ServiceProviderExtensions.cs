using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Scope;
using System.Threading.Tasks;

namespace System;

public static class ServiceProviderExtensions
{
    /// <summary>
    /// Executes <paramref name="asyncAction"/> in the specified shell's scope.
    /// </summary>
    public static async Task WithShellScopeAsync(
        this IServiceProvider serviceProvider,
        Func<ShellScope, Task> asyncAction,
        string scopeName = "Default")
    {
        var shellHost = serviceProvider.GetRequiredService<IShellHost>();
        var shellScope = await shellHost.GetScopeAsync(scopeName);
        await shellScope.UsingAsync(asyncAction);
    }

    public static async Task<T> GetWithShellScopeAsync<T>(
        this IServiceProvider serviceProvider,
        Func<ShellScope, Task<T>> asyncFunc,
        string scopeName = "Default")
    {
        T result = default;

        await serviceProvider.WithShellScopeAsync(async scope => result = await asyncFunc(scope), scopeName);

        return result;
    }
}
