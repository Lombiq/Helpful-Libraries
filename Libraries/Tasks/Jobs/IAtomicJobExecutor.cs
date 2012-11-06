using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Piedone.HelpfulLibraries.Tasks.Jobs
{
    public interface IAtomicJobExecutor : IDependency
    {
        void Run(IJob job);
    }
}
