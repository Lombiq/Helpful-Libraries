using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment.Extensions;

// Records should be in the Models namespace...
namespace Piedone.HelpfulLibraries.Models
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Locking.Database")]
    public class DatabaseLockRecord
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime? AcquiredUtc { get; set; }
    }
}
