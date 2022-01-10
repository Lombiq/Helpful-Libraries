using Orchard;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor;
using Orchard.Environment.Extensions;
using Orchard.Environment.State;
using Orchard.Events;
using System.Collections.Generic;

namespace Piedone.HelpfulLibraries.Libraries.Utilities
{
    /// <summary>
    /// Service to restart the application domain at the end of the current request.
    /// </summary>
    public interface IDeferredAppDomainRestarter : IDependency
    {
        /// <summary>
        /// Restarts the application domain at the end of the current request.
        /// </summary>
        void RestartAppDomainWhenRequestEnds();
    }

    public interface IDeferredAppDomainRestartHandler : IEventHandler
    {
        void RestartAppDomain();
    }

    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public class DeferredAppDomainRestarter : IDeferredAppDomainRestarter, IDeferredAppDomainRestartHandler
    {
        private readonly ShellSettings _shellSettings;
        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly IProcessingEngine _processingEngine;
        private readonly IWorkContextAccessor _wca;

        public DeferredAppDomainRestarter(
            ShellSettings shellSettings,
            IShellDescriptorManager shellDescriptorManager,
            IProcessingEngine processingEngine,
            IWorkContextAccessor wca)
        {
            _shellSettings = shellSettings;
            _shellDescriptorManager = shellDescriptorManager;
            _processingEngine = processingEngine;
            _wca = wca;
        }

        public void RestartAppDomainWhenRequestEnds()
        {
            _processingEngine.AddTask(
                _shellSettings,
                _shellDescriptorManager.GetShellDescriptor(),
                "IDeferredAppDomainRestartHandler.RestartAppDomain",
                new Dictionary<string, object>());
        }

        public void RestartAppDomain()
        {
            _wca.GetContext().Resolve<IHostEnvironment>().RestartAppDomain();
        }
    }
}
