using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rogero.Common.ExtensionMethods
{
    public static class EnumerableExtensionMethods
    {
        //        [DebuggerStepThrough]
        //        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        //        {
        //            foreach (var item in list)
        //            {
        //                action(item);
        //            }
        //        }
        //
        //        [DebuggerStepThrough]
        //        public static void ForEach<T>(this IEnumerable<T> list, Action<int, T> action)
        //        {
        //            int index = 0;
        //            foreach (var item in list)
        //            {
        //                action(index++, item);
        //            }
        //        }

        public static IEnumerable<T> WhereCastTo<T>(this IEnumerable list)
        {
            foreach (var item in list)
            {
                if (item is T castedItem) yield return castedItem;
            }
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                if (item != null) yield return item;
            }
        }

        public static IEnumerable<TReturn> SelectIfResultCastableTo<T, TReturn>(this IEnumerable<T> list, Func<T, object> select)
        {
            foreach (var item in list)
            {
                var result = select(item);
                if (result is TReturn castedResult) yield return castedResult;
            }
        }

        public static HashSet<T> MyToHashSet<T>(this IEnumerable<T> enumerable) => new HashSet<T>(enumerable);

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng = null)
        {
            rng = rng ?? new Random();
            
            if (source == null) throw new ArgumentNullException("source");
            if (rng == null) throw new ArgumentNullException("rng");

            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(this IEnumerable<T> source, Random rng)
        {
            var buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }

        /// <summary>
        /// An eager ForEach on an IEnumerable.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public static void MyForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
    }
}