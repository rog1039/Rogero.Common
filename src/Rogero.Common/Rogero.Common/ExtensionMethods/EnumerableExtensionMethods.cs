﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogero.Common.ExtensionMethods
{
    public static class EnumerableExtensionMethods
    {
        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> list, Action<int, T> action)
        {
            int index = 0;
            foreach (var item in list)
            {
                action(index++, item);
            }
        }

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

        public static IEnumerable<TReturn> SelectIfResultCastableTo<T,TReturn>(this IEnumerable<T> list, Func<T, object> select)
        {
            foreach (var item in list)
            {
                var result = select(item);
                if (result is TReturn castedResult) yield return castedResult;
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable) => new HashSet<T>(enumerable);
    }
}
