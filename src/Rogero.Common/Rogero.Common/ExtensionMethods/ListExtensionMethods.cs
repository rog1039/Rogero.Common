using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Rogero.Common.ExtensionMethods
{
    public static class ListExtensionMethods
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> newItems)
        {
            foreach (var newItem in newItems)
            {
                list.Add(newItem);
            }
        }
        
        public static void AddRangeParams<T>(this IList<T> list, params T[] newItems)
        {
            foreach (var newItem in newItems)
            {
                list.Add(newItem);
            }
        }

        public static void AddTo<T>(this IEnumerable<T> objects, IList<T> list)
        {
            foreach (var obj in objects)
            {
                list.Add(obj);
            }
        }

        public static IList<T> MakeList<T>(this T item)
        {
            return new List<T>() { item };
        }

        public static IList<T> GetFirstOfGroup<T, TGroup, TSort>(this IEnumerable<T> list, Func<T, TGroup> groupByFunc, Func<T,TSort> sortByFunc, SortOrder order)
        {
            var groups = list.GroupBy(groupByFunc);
            
            switch (order)
            {
                case SortOrder.Unspecified:
                    return groups.Select(g => g.First()).ToList();
                case SortOrder.Ascending:
                    return groups.Select(g => g.OrderBy(sortByFunc)).First().ToList();
                case SortOrder.Descending:
                    return groups.Select(g => g.OrderByDescending(sortByFunc).First()).ToList();
                default:
                    throw new ArgumentOutOfRangeException(nameof(order), order, null);
            }
        }

        public static bool IsEmpty<T>(this ICollection list)
        {
            return list.Count == 0;
        }

        public static bool IsEmpty<T>(this IReadOnlyCollection<T> list)
        {
            return list.Count == 0;
        }

        public static IEnumerable<T> Do<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
                yield return item;
            }
        }
    }

    public enum SortOrder
    {
        Unspecified = -1,
        Ascending = 0,
        Descending = 1,
    }
}