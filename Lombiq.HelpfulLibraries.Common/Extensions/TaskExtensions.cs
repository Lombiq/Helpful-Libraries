namespace System.Threading.Tasks;

public static class TaskExtensions
{
    /// <summary>
    /// A shortcut for <see cref="MulticastDelegateExtensions.InvokeAsync{TDelegate}"/> when the delegate is a <see
    /// cref="Task"/> returning <see cref="Func{TIn, TResult}"/>.
    /// </summary>
    public static Task InvokeFuncAsync<TIn>(this Func<TIn, Task> @delegate, TIn argument) =>
        @delegate.InvokeAsync<Func<TIn, Task>>(func => func(argument));

    /// <summary>
    /// Converts a <see cref="Task{TResult}"/> that returns an <see cref="IDisposable"/> instance into <see
    /// cref="Task"/> by disposing and discarding its result.
    /// </summary>
    public static Task DisposeResultAsync<T>(this Task<T> task)
        where T : IDisposable =>
        task.ContinueWith(
            finishedTask => finishedTask.Result?.Dispose(),
            TaskScheduler.Current);
}
