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
using Orchard.Exceptions;

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

            // This way we can create a nested transaction scope instead of having the unwanted effect of manipulating the transaction
            // of the caller.
            using (var scope = BeginLifeTimeScope(name))
            {
                var canAcquire = GetRecord(scope, name) == null;

                if (canAcquire)
                {
                    _name = name;
                    _isAcquired = true;

                    var repository = scope.Resolve<IRepository<DatabaseLockRecord>>();
                    repository.Create(new DatabaseLockRecord
                    {
                        Name = name,
                        AcquiredUtc = _clock.UtcNow
                    });
                    repository.Flush();
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
                try
                {
                    var record = GetRecord(scope, _name);
                    if (record != null)
                    {
                        var repository = scope.Resolve<IRepository<DatabaseLockRecord>>();
                        repository.Delete(record);
                        repository.Flush();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.IsFatal()) throw;
                }
            }
        }


        private ILifetimeScope BeginLifeTimeScope(string name)
        {
            var scope = _orchardHost.GetShellContext(_shellSettings).LifetimeScope.BeginLifetimeScope("Piedone.HelpfulLibraries.Tasks.Locking.Database." + name);
            scope.Resolve<ITransactionManager>().RequireNew(IsolationLevel.ReadUncommitted);
            return scope;
        }

        private DatabaseLockRecord GetRecord(ILifetimeScope scope, string name)
        {
            return scope.Resolve<IRepository<DatabaseLockRecord>>().Table.Where(record => record.Name == name).FirstOrDefault();
        }
    }
}
