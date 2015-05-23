using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment.Extensions;
using Orchard.TaskLease.Services;
using Piedone.HelpfulLibraries.Libraries.DependencyInjection;

namespace Piedone.HelpfulLibraries.Tasks
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.TaskLease")]
    public class TaskLeaseDecoratorModule : DecoratorsModuleBase
    {
        protected override IEnumerable<DecoratorsModuleBase.DecorationConfiguration> DescribeDecorators()
        {
            return new[]
            {
                new DecorationConfiguration(typeof(ITaskLeaseService), typeof(TaskLeaseServiceDecorator))
            };
        }
    }
}
