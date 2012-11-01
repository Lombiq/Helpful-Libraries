using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Tasks.Jobs
{
    public interface IJob
    {
        string Industry { get; }
        object Context { get; }
    }

    public interface IJob<T> : IJob
    {
        new T Context { get; }
    }


    //public class Job : IJob
    //{
    //    public string Industry { get; private set; }
    //    public object Context { get; private set; }

    //    public Job(string industry, object context)
    //    {
    //        Industry = industry;
    //        Context = context;
    //    }
    //}

    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Jobs")]
    public class Job<T> : IJob<T>
    {
        public string Industry { get; private set; }
        public T Context { get; private set; }
        object IJob.Context { get { return Context; } }

        public Job(string industry, T context)
        {
            Industry = industry;
            Context = context;
        }
    }
}
