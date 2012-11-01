using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Piedone.HelpfulLibraries.Tasks.Jobs
{
    public interface IAtomicJobQueue : IDependency
    {
        void Queue<TJobExecutor>(string industry) where TJobExecutor : IJobExecutor;
    }
}
