using System.Collections;

namespace GGOOF.Version3.MetadataDictionaries
{
    public sealed class GenericMultiTypeMetadataDictionary
    {
        private readonly Dictionary<string, Type> _metadataKeyTypes = new();
        private readonly Dictionary<Type, object> _metadataDictionaries = new();
        public ulong Count { get; private set; }

        public void Set<TValue>(string key, TValue value)
        {
            var dictionary = GetOrCreateDictionary<TValue>();
            var preCount = dictionary.Count;
            dictionary[key] = value;
            _metadataKeyTypes[key] = typeof(TValue);
            Count += (ulong)(dictionary.Count - preCount);
        }

        public bool TryGet<TValue>(string key, out TValue value)
        {
            value = default!;

            if (!TryGetDictionary<TValue>(out var dictionary))
                return false;

            return dictionary!.TryGetValue(key, out value!);
        }

        public bool Remove(string key)
        {
            if (!_metadataKeyTypes.TryGetValue(key, out var type))
                return false;

            if (_metadataDictionaries.TryGetValue(type, out var dictionaryObj) && 
                dictionaryObj is IDictionary dictionary)
            {
                dictionary.Remove(key);
                Count--;
                _metadataKeyTypes.Remove(key);
                return true;
            }

            return false;
        }

        private Dictionary<string, TValue> GetOrCreateDictionary<TValue>()
        {
            var type = typeof(TValue);

            if (_metadataDictionaries.TryGetValue(type, out var dictionary))
                return (Dictionary<string, TValue>)dictionary;

            dictionary = new Dictionary<string, TValue>();
            _metadataDictionaries[type] = dictionary;

            return (Dictionary<string, TValue>)dictionary;
        }

        private bool TryGetDictionary<TValue>(out Dictionary<string, TValue>? dictionary)
        {
            var type = typeof(TValue);

            if (_metadataDictionaries.TryGetValue(type, out var dict))
            {
                dictionary = (Dictionary<string, TValue>)dict;
                return true;
            }

            dictionary = null;
            return false;
        }
    }
}
