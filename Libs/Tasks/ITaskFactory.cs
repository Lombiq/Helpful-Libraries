using System;
using Orchard;
using System.Threading;
using System.Threading.Tasks;

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
        /// All parameters are optional. For documentation please refer to the documentation of the Task class.
        /// </summary>
        /// <see cref="System.Threading.Tasks.Task"/>
        Task Factory(Action action, CancellationToken cancellationToken = new CancellationToken(), TaskCreationOptions creationOptions = TaskCreationOptions.None, bool catchExceptions = true);

        /// <summary>
        /// Creates a new Task instance with the specified parameters
        /// 
        /// All parameters are optional. For documentation please refer to the documentation of the Task class.
        /// </summary>
        /// <see cref="System.Threading.Tasks.Task"/>
        Task Factory(Action<object> action, object state, CancellationToken cancellationToken = new CancellationToken(), TaskCreationOptions creationOptions = TaskCreationOptions.None, bool catchExceptions = true);
        
        /// <summary>
        /// Encapsulates the specified action so that it can safely run Orchard code
        /// 
        /// Use this if you want to instantiate Task yourself.
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="catchExceptions">If true, exceptions thrown from the action will be caught and logged (defaults to true, recommended)</param>
        /// <returns>The encapsulated action</returns>
        Action BuildTaskAction(Action action, bool catchExceptions = true);

        /// <summary>
        /// Encapsulates the specified action so that it can safely run Orchard code
        /// 
        /// Use this if you want to instantiate Task yourself.
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="catchExceptions">If true, exceptions thrown from the action will be caught and logged (defaults to true, recommended)</param>
        /// <returns>The encapsulated action</returns>
        Action<object> BuildTaskAction(Action<object> action, bool catchExceptions = true);
    }
}
