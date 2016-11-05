using System.Collections.Generic;

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
    }
}