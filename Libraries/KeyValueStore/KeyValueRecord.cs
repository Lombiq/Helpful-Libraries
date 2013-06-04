using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Data.Conventions;
using Orchard.Environment.Extensions;

// Records should be in the Models namespace...
namespace Piedone.HelpfulLibraries.Models
{
    [OrchardFeature("Piedone.HelpfulLibraries.KeyValueStore")]
    public class KeyValueRecord
    {
        public virtual int Id { get; set; }
        public virtual string StringKey { get; set; } // A column "Key" wouldn't work in the DB
        [StringLengthMax]
        public virtual string Value { get; set; }
    }
}
