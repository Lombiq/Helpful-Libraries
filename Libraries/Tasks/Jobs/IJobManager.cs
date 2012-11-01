using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Piedone.HelpfulLibraries.Tasks.Jobs
{
    public interface IJobManager : IDependency
    {
        void CreateJob(string industry, object context);
        IJob<T> TakeJob<T>(string industry);
        void Done(IJob job);
        void GiveBack(IJob job);
    }
}
