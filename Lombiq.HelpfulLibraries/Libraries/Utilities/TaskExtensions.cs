using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Libraries.Utilities
{
    public static class TaskExtensions
    {
        [SuppressMessage(
            "Usage",
            "VSTHRD003",
            MessageId = "Avoid awaiting foreign Tasks",
            Justification = "It's always safe because one of the tasks is local.")]
        public static Task WithTimeoutAsync(this Task task, TimeSpan timeout, CancellationToken cancellationToken = default) =>
            Task.WhenAny(task, Task.Delay(timeout, cancellationToken));

        [SuppressMessage(
            "Usage",
            "VSTHRD003",
            MessageId = "Avoid awaiting foreign Tasks",
            Justification = "It's always safe because one of the tasks is local.")]
        public static async Task<T> WithTimeoutAsync<T>(this Task<T> task, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            await Task.WhenAny(task, Task.Delay(timeout, cancellationToken));
            return task.IsCompleted ? await task : default;
        }
    }
}
