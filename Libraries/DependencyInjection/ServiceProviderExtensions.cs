using Lombiq.HelpfulLibraries.Libraries.Utilities;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell.Scope;

namespace System
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Creates a new service scope under the <paramref name="serviceProvider"/> and wraps it for safe concurrent
        /// disposal if true concurrency can not be avoided. Necessary becuase the <see cref="ShellScope"/>
        /// implementation is not thread safe.
        /// </summary>
        public static LockingDisposable<IServiceScope> CreateConcurrentScope(this IServiceProvider serviceProvider) =>
            new LockingDisposable<IServiceScope>(serviceProvider.CreateScope());
    }
}
