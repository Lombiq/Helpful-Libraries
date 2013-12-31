using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Data;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Autofac;
using Orchard.Environment.Extensions;
using System.Data;
using Orchard.Services;
using Piedone.HelpfulLibraries.Models;
using Orchard;

namespace Piedone.HelpfulLibraries.Tasks.Locking
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Locking.Database")]
    public class DatabaseLock : IDistributedLock
    {
        private readonly IOrchardHost _orchardHost;
        private readonly ShellSettings _shellSettings;
        private readonly IClock _clock;
        private string _name;
        private bool _isDisposed = false;
        private bool _isAcquired = false;


        public DatabaseLock(
            IOrchardHost orchardHost,
            ShellSettings shellSettings,
            IClock clock)
        {
            _orchardHost = orchardHost;
            _shellSettings = shellSettings;
            _clock = clock;
        }


        public bool TryAcquire(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (name.Length > 255) throw new ArgumentException("The lock's name can't be longer than 255 characters.");

            using (var scope = BeginLifeTimeScope(name))
            {
                var canAcquire = GetRecordInTransaction(scope, name) == null;

                if (canAcquire)
                {
                    _name = name;
                    _isAcquired = true;

                    scope.Resolve<IRepository<DatabaseLockRecord>>().Create(new DatabaseLockRecord
                    {
                        Name = name,
                        AcquiredUtc = _clock.UtcNow
                    });
                }

                return canAcquire;
            }
        }

        // This will be called at least by Autofac when the request ends
        public void Dispose()
        {
            if (_isDisposed || !_isAcquired) return;

            _isDisposed = true;

            using (var scope = BeginLifeTimeScope(_name))
            {
                var record = GetRecordInTransaction(scope, _name);
                if (record != null)
                {
                    scope.Resolve<IRepository<DatabaseLockRecord>>().Delete(record);
                }
            }
        }


        private ILifetimeScope BeginLifeTimeScope(string name)
        {
            return _orchardHost.GetShellContext(_shellSettings).LifetimeScope.BeginLifetimeScope("Piedone.HelpfulLibraries.Tasks.Locking.Database." + name);
        }

        private DatabaseLockRecord GetRecordInTransaction(ILifetimeScope scope, string name)
        {
            var transactionManager = scope.Resolve<ITransactionManager>();
            transactionManager.RequireNew(IsolationLevel.ReadUncommitted);
            var repository = scope.Resolve<IRepository<DatabaseLockRecord>>();

            return repository.Table.Where(record => record.Name == name).SingleOrDefault();
        }
    }
}
