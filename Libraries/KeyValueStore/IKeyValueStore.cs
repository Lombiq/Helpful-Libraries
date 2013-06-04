using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Piedone.HelpfulLibraries.KeyValueStore
{
    public interface IKeyValueStore : IDependency
    {
        bool Exists(string key);
        void Set(string key, object value);
        T Get<T>(string key);
        void Remove(string key);
    }
}
