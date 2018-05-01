using System.Collections.Generic;

namespace Rogero.Common.ExtensionMethods
{
    public static class ISetExtensionMethods
    {
        public static void AddRange<T>(this ISet<T> set, IEnumerable<T> objects)
        {
            foreach (var obj in objects)
            {
                set.Add(obj);
            }
        }
    }
}