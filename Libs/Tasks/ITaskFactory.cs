using System;
using Orchard;

namespace Piedone.HelpfulLibraries.Tasks
{
    public interface ITaskFactory : ISingletonDependency
    {
        System.Threading.Tasks.Task Factory(Action action, bool catchExceptions = true);
        Orchard.Logging.ILogger Logger { get; set; }
    }
}
