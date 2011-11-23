using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Settings;
using System.Web;
using Orchard;
using System.Transactions;
using Orchard.Logging;
using System.Threading.Tasks;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Tasks
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks")]
    public class TaskFactory : ITaskFactory
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public ILogger Logger { get; set; }

        private class TaskContext
        {
            public string CurrentCulture { get; private set; }
            public ISite CurrentSite { get; private set; }
            public HttpContextBase HttpContext { get; private set; }

            public TaskContext(WorkContext workContext)
            {
                CurrentCulture = workContext.CurrentCulture;
                CurrentSite = workContext.CurrentSite;
                //HttpContext = new HttpContextPlaceholder();
                HttpContext = workContext.HttpContext;
            }

            public WorkContext Transcribe(WorkContext workContext)
            {
                workContext.CurrentCulture = CurrentCulture;
                workContext.CurrentSite = CurrentSite;
                workContext.HttpContext = HttpContext;

                return workContext;
            }
        }

        public TaskFactory(
            IWorkContextAccessor workContextAcessor
            )
        {
            _workContextAccessor = workContextAcessor;

            Logger = NullLogger.Instance; // Constructor injection of ILogger fails
        }

        public Task Factory(Action action, bool catchExceptions = true)
        {
            return new Task(BuildTaskAction(action, catchExceptions));
        }

        private Action BuildTaskAction(Action action, bool catchExceptions)
        {
            var taskContext = new TaskContext(_workContextAccessor.GetContext());

            return () =>
            {
                using (var scope = _workContextAccessor.CreateWorkContextScope())
                {
                    using (var transactionScope = new TransactionScope(TransactionScopeOption.Required))
                    {
                        ((TaskContext)taskContext).Transcribe(_workContextAccessor.GetContext());

                        if (catchExceptions)
                        {
                            try
                            {
                                action();
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e, "Background task failed with exception " + e.Message);
                            } 
                        }
                        else
                        {
                            action();
                        }
                    }
                }
            };
        }
    }
}
