using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Libraries.Utilities
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Returns a <see cref="Task"/> that completes either if the provided <paramref name="task"/> is completed or
        /// if the time indicated by <paramref name="timeout"/> has elapsed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that if the <paramref name="timeout"/> is up, that won't cancel the <paramref name="task"/> because
        /// this method doesn't have access to its cancellation token source, if there is any. However if one exists you
        /// can manually trigger cancellation after checking if <see cref="Task.IsCompleted"/> is <see langword="false"/>
        /// on the <paramref name="task"/>.
        /// </para>
        /// </remarks>
        [SuppressMessage(
            "Usage",
            "VSTHRD003",
            MessageId = "Avoid awaiting foreign Tasks",
            Justification = "It's always safe because one of the tasks is local.")]
        public static Task WithTimeoutAsync(this Task task, TimeSpan timeout, CancellationToken cancellationToken = default) =>
            Task.WhenAny(task, Task.Delay(timeout, cancellationToken));

        /// <summary>
        /// Returns a <see cref="Task"/> that resolves the <see cref="Task{TResult}.Result"/> of <paramref name="task"/>
        /// if it completed, or <see langword="default"/> of <typeparamref name="T"/> if the time indicated by
        /// <paramref name="timeout"/> has elapsed.
        /// </summary>
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
