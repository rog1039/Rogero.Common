using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rogero.Common.ExtensionMethods
{
    public static class ObservableCollectionExtensionMethods
    {
        public static void ReplaceObservableCollectionItems<T>(this IEnumerable<T> items, ObservableCollection<T> oc)
        {
            oc.Clear();
            foreach (var item in items)
            {
                oc.Add(item);
            }
        }
    }
}
