using System;
using System.Diagnostics.CodeAnalysis;

namespace Lombiq.HelpfulLibraries.Libraries.Utilities
{
    /// <summary>
    /// A container for <see cref="IDisposable"/> whose dispose logic is not thread safe.
    /// </summary>
    public sealed class LockingDisposable<T> : IDisposable
        where T : IDisposable
    {
        [SuppressMessage("Code Smell", "S2743", Justification = "Wouldn't work without it.")]
        private static readonly object _lock = new();

        public T Value { get; }

        public LockingDisposable(T value) => Value = value;

        public void Dispose()
        {
            lock (_lock) Value?.Dispose();
        }
    }
}
