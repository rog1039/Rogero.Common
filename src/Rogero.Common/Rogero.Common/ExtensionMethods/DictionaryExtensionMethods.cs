using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Rogero.Options;

namespace Rogero.Common.ExtensionMethods
{
    public static class DictionaryExtensionMethods
    {
        public static Dictionary<K, IList<T>> ToDictionaryMany<T, K>(this IEnumerable<T> list, Func<T, K> keyFunc)
        {
            var dict = new Dictionary<K, IList<T>>();
            foreach (var item in list)
            {
                dict.AddToDictionary(keyFunc(item), item);
            }
            return dict;
        }

        public static Dictionary<K, IList<V>> ToDictionaryMany<T, K, V>(this IEnumerable<T> list, Func<T, K> keyFunc,
            Func<T, V> valueFunc)
        {
            var dict = new Dictionary<K, IList<V>>();
            foreach (var item in list)
            {
                dict.AddToDictionary(keyFunc(item), valueFunc(item));
            }
            return dict;
        }
        
        public static Dictionary<K, T> ToDictionaryAggregate<T, K>(this IEnumerable<T> list, Func<T, K> keyFunc,
            Func<T,T,T> valueAggregationFunc)
        {
            var dict = new Dictionary<K, T>();
            foreach (var item in list)
            {
                var key = keyFunc(item);
                if (dict.TryGetValue(key, out var val))
                {
                    var newVal = valueAggregationFunc(val, item);
                    dict[key] = newVal;
                }
                else
                {
                    dict[key] = item;
                }
            }
            return dict;
        }

        /// <summary>
        /// Just like the built in ToDictionary but this will report what the duplicate key was if it exists.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="keyFunc"></param>
        /// <param name="valueAggregationFunc"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <returns></returns>
        public static Dictionary<K, T> ToDictionaryExplain<T, K>(this IEnumerable<T> list, Func<T, K> keyFunc)
        {
            var dict = new Dictionary<K, T>();
            foreach (var item in list)
            {
                var key = keyFunc(item);
                if (dict.TryGetValue(key, out var val))
                {
                    throw new InvalidOperationException($"Duplicate key found: {key}");
                }
                else
                {
                    dict.Add(key, item);
                }
            }
            return dict;
        }


        public static SortedDictionary<K, IList<V>> ToSortedDictionaryMany<T, K, V>(this IEnumerable<T> list,
            Func<T, K> keyFunc,
            Func<T, V> valueFunc)
        {
            var dict = new SortedDictionary<K, IList<V>>();
            foreach (var item in list)
            {
                dict.AddToDictionary(keyFunc(item), valueFunc(item));
            }
            return dict;
        }

        public static SortedDictionary<K, IList<T>> ToSortedDictionaryMany<T, K>(this IEnumerable<T> list,
            Func<T, K> keyFunc)
        {
            var dict = new SortedDictionary<K, IList<T>>();
            foreach (var item in list)
            {
                dict.AddToDictionary(keyFunc(item), item);
            }
            return dict;
        }

        public static void AddToDictionary<K, V>(this IDictionary<K, IList<V>> dict, K key, V value)
        {
            var existingList = dict.TryGetValue(key);
            if (existingList.HasValue)
                existingList.Value.Add(value);
            else
                dict.Add(key, new List<V>() {value});
        }

        public static void AddRange<K, V>(this IDictionary<K, IList<V>> dict, IList<V> values, Func<V, K> keyFunc)
        {
            foreach (var value in values)
            {
                dict.AddToDictionary(keyFunc(value), value);
            }
        }

        public static void AddRange<T, K, V>(this IDictionary<K, IList<V>> dict, IList<T> items, Func<T, K> keyFunc,
            Func<T, V> valueFunc)
        {
            foreach (var item in items)
            {
                dict.AddToDictionary(keyFunc(item), valueFunc(item));
            }
        }
    }
}