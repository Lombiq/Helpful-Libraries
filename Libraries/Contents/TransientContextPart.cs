using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement;

namespace Piedone.HelpfulLibraries.Contents
{
    public class TransientContextPart : ContentPart
    {
        internal Dictionary<string, object> Context { get; set; }


        public TransientContextPart()
        {
            Context = new Dictionary<string, object>();
        }
    }
}
