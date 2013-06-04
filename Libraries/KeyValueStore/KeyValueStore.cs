using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Services;
using Piedone.HelpfulLibraries.Models;

namespace Piedone.HelpfulLibraries.KeyValueStore
{
    [OrchardFeature("Piedone.HelpfulLibraries.KeyValueStore")]
    public class KeyValueStore : IKeyValueStore
    {
        private readonly IRepository<KeyValueRecord> _repository;
        private readonly IJsonConverter _jsonConverter;


        public KeyValueStore(IRepository<KeyValueRecord> repository, IJsonConverter jsonConverter)
        {
            _repository = repository;
            _jsonConverter = jsonConverter;
        }
	
			
        public bool Exists(string key)
        {
            ThrowIfKeyNull(key);

            return GetRecord(key) != null;
        }

        public void Set(string key, object value)
        {
            ThrowIfKeyNull(key);
            if (value == null) throw new ArgumentNullException("value");
            if (key.Length > 2048) throw new ArgumentException("The key can't be longer than 2048 characters");

            var serialized = _jsonConverter.Serialize(value);
            var record = GetRecord(key);
            if (record == null)
            {
                record = new KeyValueRecord { StringKey = key, Value = serialized };
                _repository.Create(record);
            }
            else record.Value = serialized;
        }

        public T Get<T>(string key)
        {
            ThrowIfKeyNull(key);

            var record = GetRecord(key);
            if (record == null) return default(T);
            return _jsonConverter.Deserialize<T>(record.Value);
        }

        public void Remove(string key)
        {
            ThrowIfKeyNull(key);

            var record = GetRecord(key);
            if (record == null) return;
            _repository.Delete(record);
        }


        private KeyValueRecord GetRecord(string key)
        {
            ThrowIfKeyNull(key);
            return _repository.Table.Where(record => record.StringKey == key).SingleOrDefault();
        }


        private static void ThrowIfKeyNull(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
        }
    }
}
