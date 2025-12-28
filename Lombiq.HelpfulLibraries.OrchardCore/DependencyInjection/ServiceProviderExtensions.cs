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
    public static Task WithShellScopeAsync(
        this IServiceProvider serviceProvider,
        Func<ShellScope, Task> asyncAction,
        string scopeName = ShellSettings.DefaultShellName) =>
        serviceProvider.GetRequiredService<IShellHost>().WithShellScopeAsync(asyncAction, scopeName);

    /// <summary>
    /// Executes <paramref name="asyncFunc"/> in the specified shell's scope and returns the resulting object.
    /// </summary>
    public static Task<T?> GetWithShellScopeAsync<T>(
        this IServiceProvider serviceProvider,
        Func<ShellScope, Task<T>> asyncFunc,
        string scopeName = ShellSettings.DefaultShellName) =>
        serviceProvider.GetRequiredService<IShellHost>().GetWithShellScopeAsync(asyncFunc, scopeName);
}
