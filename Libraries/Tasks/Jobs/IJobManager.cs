using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Tasks.Jobs
{
    public interface IJobManager : IDependency
    {
        void CreateJob(string industry, object context);
        IJob TakeJob(string industry);
        void Done(IJob job);
        void GiveBack(IJob job);
    }

    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Jobs")]
    public static class JobManagerExtensions
    {
        public static IJob<TContext> TakeJob<TContext>(this IJobManager jobManager, string industry)
        {
            var job = jobManager.TakeJob(industry);

            return new Job<TContext>(job.Industry, (TContext)job.Context);
        }
    }
}
