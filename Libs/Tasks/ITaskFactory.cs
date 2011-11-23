using System;
using Orchard;
using System.Threading;
using System.Threading.Tasks;

namespace Piedone.HelpfulLibraries.Tasks
{
    public interface ITaskFactory : ISingletonDependency
    {
        Task Factory(Action action, CancellationToken cancellationToken = new CancellationToken(), TaskCreationOptions creationOptions = TaskCreationOptions.None, bool catchExceptions = true);
        Task Factory(Action<object> action, object state, CancellationToken cancellationToken = new CancellationToken(), TaskCreationOptions creationOptions = TaskCreationOptions.None, bool catchExceptions = true);
        Orchard.Logging.ILogger Logger { get; set; }
    }
}
