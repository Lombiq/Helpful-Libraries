using OrchardCore.Environment.Shell.Scope;
using System;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell;

public static class ShellHostExtensions
{
    /// <summary>
    /// Executes <paramref name="asyncAction"/> in the specified shell's scope.
    /// </summary>
    public static async Task WithShellScopeAsync(
        this IShellHost shellHost,
        Func<ShellScope, Task> asyncAction,
        string scopeName = ShellSettings.DefaultShellName)
    {
        var shellScope = await shellHost.GetScopeAsync(scopeName);
        await shellScope.UsingAsync(asyncAction);
        await shellScope.DisposeAsync();
    }

    /// <summary>
    /// Executes <paramref name="asyncFunc"/> in the specified shell's scope and returns the resulting object.
    /// </summary>
    public static async Task<T> GetWithShellScopeAsync<T>(
        this IShellHost shellHost,
        Func<ShellScope, Task<T>> asyncFunc,
        string scopeName = ShellSettings.DefaultShellName)
    {
        T result = default;

        await shellHost.WithShellScopeAsync(async scope => result = await asyncFunc(scope), scopeName);

        return result;
    }
}
