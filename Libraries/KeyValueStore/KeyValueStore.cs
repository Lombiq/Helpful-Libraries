using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Caching;
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
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;


        public KeyValueStore(
            IRepository<KeyValueRecord> repository,
            IJsonConverter jsonConverter,
            ICacheManager cacheManager,
            ISignals signals)
        {
            _repository = repository;
            _jsonConverter = jsonConverter;
            _cacheManager = cacheManager;
            _signals = signals;
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
                Trigger(key);
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
            Trigger(key);
        }


        private KeyValueRecord GetRecord(string key)
        {
            ThrowIfKeyNull(key);

            // The record can't be cached directly, but fetching by ID lets the second-level cache work.
            var id = _cacheManager.Get(CacheKey(key), ctx =>
                {
                    ctx.Monitor(_signals.When(CacheKey(key)));
                    var record = _repository.Table.Where(r => r.StringKey == key).SingleOrDefault();
                    if (record == null) return 0;
                    return record.Id;
                });

            return _repository.Get(id);
        }

        private void Trigger(string key)
        {
            _signals.Trigger(CacheKey(key));
        }


        private static void ThrowIfKeyNull(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
        }

        private static string CacheKey(string key)
        {
            return "Piedone.HelpfulLibraries.KeyValueStore." + key + ".Id";
        }
    }
}
