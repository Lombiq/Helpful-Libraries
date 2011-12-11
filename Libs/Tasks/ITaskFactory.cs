using System;
using System.Threading;
using System.Threading.Tasks;
using Orchard;

namespace Piedone.HelpfulLibraries.Tasks
{
    /// <summary>
    /// Creates Task objects that work well with the Orchard ecosystem
    /// </summary>
    public interface ITaskFactory : ISingletonDependency
    {
        /// <summary>
        /// Creates a new Task instance with the specified parameters
        /// 
        /// Other than action all parameters are optional. For documentation please refer to the documentation of the Task class.
        /// </summary>
        /// <param name="catchExceptions">
        /// If true, exceptions thrown from the action will be caught and logged (defaults to true).
        /// If you opt to false, be extremely cautious to catch every possible exception in your code as any uncaught
        /// exception in a background thread causes the whole site to halt!
        /// </param>
        /// <see cref="System.Threading.Tasks.Task"/>
        Task Factory(Action action, CancellationToken cancellationToken = new CancellationToken(), TaskCreationOptions creationOptions = TaskCreationOptions.None, bool catchExceptions = true);

        /// <summary>
        /// Creates a new Task instance with the specified parameters
        /// 
        /// Other than action and state all parameters are optional. For documentation please refer to the documentation of the Task class.
        /// </summary>
        /// <param name="catchExceptions">
        /// If true, exceptions thrown from the action will be caught and logged (defaults to true).
        /// If you opt to false, be extremely cautious to catch every possible exception in your code as any uncaught
        /// exception in a background thread causes the whole site to halt!
        /// </param>
        /// <see cref="System.Threading.Tasks.Task"/>
        Task Factory(Action<object> action, object state, CancellationToken cancellationToken = new CancellationToken(), TaskCreationOptions creationOptions = TaskCreationOptions.None, bool catchExceptions = true);
        
        /// <summary>
        /// Encapsulates the specified action so that it can safely run Orchard code in background
        /// 
        /// Use this if you want to instantiate Task yourself.
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="catchExceptions">
        /// If true, exceptions thrown from the action will be caught and logged (defaults to true).
        /// If you opt to false, be extremely cautious to catch every possible exception in your code as any uncaught
        /// exception in a background thread causes the whole site to halt!
        /// </param>
        /// <returns>The encapsulated action</returns>
        Action BuildBackgroundAction(Action action, bool catchExceptions = true);

        /// <summary>
        /// Encapsulates the specified action so that it can safely run Orchard code in background
        /// 
        /// Use this if you want to instantiate Task yourself.
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="catchExceptions">
        /// If true, exceptions thrown from the action will be caught and logged (defaults to true).
        /// If you opt to false, be extremely cautious to catch every possible exception in your code as any uncaught
        /// exception in a background thread causes the whole site to halt!
        /// </param>
        /// <returns>The encapsulated action</returns>
        Action<object> BuildBackgroundAction(Action<object> action, bool catchExceptions = true);
    }
}
