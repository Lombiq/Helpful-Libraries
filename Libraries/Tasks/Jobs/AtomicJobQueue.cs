using System;
using System.Collections.Generic;
using Orchard;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor;
using Orchard.Environment.Extensions;
using Orchard.Environment.State;
using Orchard.Events;
using Orchard.Exceptions;
using Orchard.Logging;
using Piedone.HelpfulLibraries.DependencyInjection;

namespace Piedone.HelpfulLibraries.Tasks.Jobs
{
    public interface IAtomicJobExecutor : IEventHandler
    {
        void Execute(string industry, Func<WorkContext, IAtomicWorker> executorResolver);
    }

    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Jobs")]
    public class AtomicJobQueue : IAtomicJobQueue, IAtomicJobExecutor
    {
        private readonly IProcessingEngine _processingEngine;
        private readonly ShellSettings _shellSettings;
        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly IResolve<IJobManager> _jobManagerResolve;
        private readonly IWorkContextAccessor _wca;

        public ILogger Logger { get; set; }


        public AtomicJobQueue(
            IProcessingEngine processingEngine,
            ShellSettings shellSettings,
            IShellDescriptorManager shellDescriptorManager,
            IResolve<IJobManager> jobManagerResolve,
            IWorkContextAccessor wca)
        {
            _processingEngine = processingEngine;
            _shellSettings = shellSettings;
            _shellDescriptorManager = shellDescriptorManager;
            _jobManagerResolve = jobManagerResolve;
            _wca = wca;

            Logger = NullLogger.Instance;
        }


        public void Execute(string industry, Func<WorkContext, IAtomicWorker> executorResolver)
        {
            var jobManager = _jobManagerResolve.Value;
            IJob job = null;

            try
            {
                job = jobManager.TakeJob(industry);

                if (job == null) return;

                executorResolver(_wca.GetContext()).WorkOn(job);

                jobManager.Done(job);
            }
            catch (Exception ex)
            {
                if (ex.IsFatal()) throw;

                jobManager.GiveBack(job);
                Queue(industry, executorResolver);
                Logger.Error(ex, "Exception during the execution of an atomic job. The job was re-queued.");
            }
        }

        public void Queue<TAtomicWorker>(string industry) where TAtomicWorker : IAtomicWorker
        {
            Queue(industry, workContext => workContext.Resolve<TAtomicWorker>());
        }


        private void Queue(string industry, Func<WorkContext, IAtomicWorker> executorResolver)
        {
            _processingEngine.AddTask(
                _shellSettings,
                _shellDescriptorManager.GetShellDescriptor(),
                "IAtomicJobExecutor.Execute",
                new Dictionary<string, object> { { "industry", industry }, { "executorResolver", executorResolver } }
            );
        }
    }
}
