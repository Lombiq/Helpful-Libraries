using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Data;
using Orchard.Environment;
using Orchard.Environment.Extensions;
using Orchard.Services;
using Piedone.HelpfulLibraries.Models;

namespace Piedone.HelpfulLibraries.Libraries.Tasks.Locking
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Locking.Database")]
    public class LockRecordCleaner : IOrchardShellEvents
    {
        private readonly IClock _clock;
        private readonly IRepository<DatabaseLockRecord> _repository;


        public LockRecordCleaner(
            IClock clock,
            IRepository<DatabaseLockRecord> repository)
        {
            _clock = clock;
            _repository = repository;
        }


        public void Activated()
        {
            // If there are lock records on shell start older than one minute they were surely created before the shell startup,
            // thus are remainders of an earlier crash.
            foreach (var record in _repository.Table.Where(record => _clock.UtcNow.Subtract(record.AcquiredUtc.Value).Minutes > 1))
            {
                _repository.Delete(record);
            }
        }

        public void Terminating()
        {
        }
    }
}
