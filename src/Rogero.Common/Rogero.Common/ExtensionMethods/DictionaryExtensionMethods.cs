using Optional.Collections;
using Optional.Unsafe;

namespace Rogero.Common.ExtensionMethods;

#nullable enable

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

   public static Dictionary<K1, Dictionary<K2, IList<T>>> ToTwoLevelDictionaryMany<T, K1, K2>(
      this IEnumerable<T> list,
      Func<T, K1>         key1Selector,
      Func<T, K2>         key2Selector
   )
   {
      var mainDict = new Dictionary<K1, Dictionary<K2, IList<T>>>();
      foreach (var item in list)
      {
         var k1 = key1Selector(item);
         var k2 = key2Selector(item);

         if (mainDict.TryGetValue(k1, out var subdict))
         {
            if (subdict.TryGetValue(k2, out var k2List))
            {
               k2List.Add(item);
            }
            else
            {
               subdict.Add(k2, item.MakeList());
            }
         }
         else
         {
            subdict = new Dictionary<K2, IList<T>>() {{k2, item.MakeList()}};
            mainDict.Add(k1, subdict);
         }
      }

      return mainDict;
   }

   public static void Add<T, K1, K2>(this Dictionary<K1, Dictionary<K2, IList<T>>> dict, K1 k1, K2 k2, T item)
   {
      if (dict.TryGetValue(k1, out var subdict))
      {
         if (subdict.TryGetValue(k2, out var k2List))
         {
            k2List.Add(item);
         }
         else
         {
            subdict.Add(k2, item.MakeList());
         }
      }
      else
      {
         subdict = new Dictionary<K2, IList<T>>() {{k2, item.MakeList()}};
         dict.Add(k1, subdict);
      }
   }

   public static IList<T> Get<T, K1, K2>(this Dictionary<K1, Dictionary<K2, IList<T>>> dict, K1 k1, K2 k2)
   {
      return dict[k1][k2];
   }

   public static Dictionary<K, IList<V>> ToDictionaryMany<T, K, V>(this IEnumerable<T> list, Func<T, K> keyFunc,
                                                                   Func<T, V>          valueFunc)
   {
      var dict = new Dictionary<K, IList<V>>();
      foreach (var item in list)
      {
         dict.AddToDictionary(keyFunc(item), valueFunc(item));
      }

      return dict;
   }

   public static Dictionary<K, T> ToDictionaryAggregate<T, K>(this IEnumerable<T> list,
                                                              Func<T, K>          keyFunc,
                                                              Func<T, T, T>       valueAggregationFunc)
   {
      var dict = new Dictionary<K, T>();
      foreach (var item in list)
      {
         var key = keyFunc(item);
         if (dict.TryGetValue(key, out var existingItem))
         {
            var combinedItem = valueAggregationFunc(existingItem, item);
            dict[key] = combinedItem;
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

   public static Dictionary<K, T> ToDictionaryIgnoreDuplicates<T, K>(this IEnumerable<T> list, Func<T, K> keyFunc)
   {
      var dict = new Dictionary<K, T>();
      foreach (var item in list)
      {
         var key = keyFunc(item);
         if (dict.TryGetValue(key, out var val))
         {
            //Do nothing...
         }
         else
         {
            dict.Add(key, item);
         }
      }

      return dict;
   }


   public static SortedDictionary<K, IList<V>> ToSortedDictionaryMany<T, K, V>(this IEnumerable<T> list,
                                                                               Func<T, K>          keyFunc,
                                                                               Func<T, V>          valueFunc)
   {
      var dict = new SortedDictionary<K, IList<V>>();
      foreach (var item in list)
      {
         dict.AddToDictionary(keyFunc(item), valueFunc(item));
      }

      return dict;
   }

   public static SortedDictionary<K, IList<T>> ToSortedDictionaryMany<T, K>(this IEnumerable<T> list,
                                                                            Func<T, K>          keyFunc)
   {
      var dict = new SortedDictionary<K, IList<T>>();
      foreach (var item in list)
      {
         dict.AddToDictionary(keyFunc(item), item);
      }

      return dict;
   }

   public static void AddToDictionary<K, V>(this IDictionary<K, IList<V>> dict, K key, V newValue)
   {
      var existingListOption = dict.GetValueOrNone(key);
      existingListOption.Match(existingList => existingList.Add(newValue),
         () => dict.Add(key, new List<V>() {newValue}));
   }

   public static void AddRange<K, V>(this IDictionary<K, IList<V>> dict, IList<V> values, Func<V, K> keyFunc)
   {
      foreach (var value in values)
      {
         dict.AddToDictionary(keyFunc(value), value);
      }
   }

   public static void AddRange<T, K, V>(this IDictionary<K, IList<V>> dict, IList<T> items, Func<T, K> keyFunc,
                                        Func<T, V>                    valueFunc)
   {
      foreach (var item in items)
      {
         dict.AddToDictionary(keyFunc(item), valueFunc(item));
      }
   }

   public static DictionaryResult<TValue> TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
   {
      if (dict.TryGetValue(key, out var val))
      {
         return DictionaryResult<TValue>.Found(val);
      }

      return DictionaryResult<TValue>.NotFound();
   }

   public static TValue? GetValueOr<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue? orValue) where TValue : struct
   {
      var val = dict.GetValueOrNone(key);
      if (val.HasValue) return val.ValueOrFailure();
      return orValue;
   }
   public static TValue? GetValueOr<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue? orValue) where TValue : class
   {
      var val = dict.GetValueOrNone(key);
      if (val.HasValue) return val.ValueOrFailure();
      return orValue;
   }

   public static void MatchSome<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Action<TValue> action)
   {
      if (dict.TryGetValue(key, out var val))
         action(val);
   }
}

public class DictionaryResult<T>
{
   private DictionaryResult()
   {
      WasFound = false;
   }

   private DictionaryResult(T value)
   {
      WasFound = true;
      Value    = value;
   }

   public bool WasFound { get; }
   public T?   Value    { get; }

   public static DictionaryResult<T> Found(T value)
   {
      var result = new DictionaryResult<T>(value);
      return result;
   }

   public static DictionaryResult<T> NotFound()
   {
      return new DictionaryResult<T>();
   }
}