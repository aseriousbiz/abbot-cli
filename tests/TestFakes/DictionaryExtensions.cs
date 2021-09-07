using System;
using System.Collections.Generic;

namespace UnitTests
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrCreate<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary, TKey key,
            Func<TKey, TValue> creator)
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = creator(key);
                dictionary.Add(key, value);
            }

            return value;
        }
    }
}