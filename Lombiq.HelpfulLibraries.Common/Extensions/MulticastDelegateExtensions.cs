using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System;

public static class MulticastDelegateExtensions
{
    /// <summary>
    /// Invokes all async delegates in the given <see cref="MulticastDelegate"/> sequentially.
    /// </summary>
    /// <typeparam name="TDelegate">Type of the delegates in the given <see cref="MulticastDelegate"/>.</typeparam>
    /// <param name="delegateExecution">The function to execute each delegate with.</param>
    /// <returns>The <see cref="Task"/> that'll complete when all items have completed.</returns>
    public static Task InvokeAsync<TDelegate>(
        this MulticastDelegate multicastDelegate,
        Func<TDelegate, Task> delegateExecution)
        where TDelegate : Delegate =>
        multicastDelegate == null
            ? Task.CompletedTask
            : multicastDelegate.GetInvocationList()
                .Cast<TDelegate>()
                .AwaitEachAsync(delegateExecution);
}
