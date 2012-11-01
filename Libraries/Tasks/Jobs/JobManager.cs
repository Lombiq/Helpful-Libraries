using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Piedone.HelpfulLibraries.DependencyInjection;
using Piedone.HelpfulLibraries.Models;

namespace Piedone.HelpfulLibraries.Tasks.Jobs
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Jobs")]
    public class JobManager : IJobManager
    {
        private readonly IRepository<JobRecord> _repository;
        private readonly IResolve<ILockFile> _lockFileResolve;
        private readonly Dictionary<IJob, JobReference> _jobReferences = new Dictionary<IJob,JobReference>();


        public JobManager(IRepository<JobRecord> repository, IResolve<ILockFile> lockFileResolve)
        {
            _repository = repository;
            _lockFileResolve = lockFileResolve;
        }


        public void CreateJob(string industry, object context)
        {
            if (String.IsNullOrEmpty(industry)) throw new ArgumentNullException("industry");

            var record = new JobRecord
            {
                Industry = industry,
                ContextDefinion = JsonConvert.SerializeObject(context)
            };

            _repository.Create(record);
            _repository.Flush();
        }

        public IJob<T> TakeJob<T>(string industry)
        {
            var jobs = _repository.Table.Where(record => record.Industry == industry);
            var jobCount = jobs.Count();

            if (jobCount == 0) return null;

            var lockFile = _lockFileResolve.Value;
            var jobNumber = 0;

            while (!lockFile.TryAcquire("Job - " + industry + jobNumber) && jobNumber < jobCount)
            {
                jobNumber++;
            }

            // All the jobs are locked, nothing to do
            if (jobNumber == jobCount) return null;

            var jobRecord = jobs.Skip(jobNumber).Take(1).Single();

            var job = new Job<T>
            (
                industry,
                JsonConvert.DeserializeObject<T>(jobRecord.ContextDefinion)
            );
            _jobReferences[job] = new JobReference { Id = jobRecord.Id, LockFile = lockFile };

            return job;
        }

        public void Done(IJob job)
        {
            if (!_jobReferences.ContainsKey(job)) return;

            var jobReference = _jobReferences[job];
            jobReference.LockFile.Dispose();
            _repository.Delete(_repository.Get(jobReference.Id));
            _repository.Flush();
            _jobReferences.Remove(job);
        }

        public void GiveBack(IJob job)
        {
            if (!_jobReferences.ContainsKey(job)) return;

            _jobReferences[job].LockFile.Dispose();
            _jobReferences.Remove(job);
        }

        // No need to dispose undisposed jobs' lock files, as the Dispose() on the lock files will be called by Autofac
        //public void Dispose()
        //{
        //    // Disposing every undisposed lock file
        //    foreach (var lockFile in _jobReferences.Values.Select(reference => reference.LockFile))
        //    {
        //        lockFile.Dispose();
        //    }
        //}


        private class JobReference
        {
            public int Id { get; set; }
            public ILockFile LockFile { get; set; }
        }
    }
}
