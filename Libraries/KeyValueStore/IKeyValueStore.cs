using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Piedone.HelpfulLibraries.KeyValueStore
{
    /// <summary>
    /// A simple persistent key-value store
    /// </summary>
    public interface IKeyValueStore : IDependency
    {
        /// <summary>
        /// Checks if an entry with the specified key exists
        /// </summary>
        bool Exists(string key);

        /// <summary>
        /// Sets the value of an entry with the specified key. If the entry doesn't exists it creates it, if it exists, overwrites it.
        /// </summary>
        void Set(string key, object value);

        /// <summary>
        /// Gets the value of the entry with the specified key
        /// </summary>
        /// <typeparam name="T">Type of the value that was saved</typeparam>
        T Get<T>(string key);

        /// <summary>
        /// Removes the entry with the specified key
        /// </summary>
        void Remove(string key);
    }
}
