using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rogero.Common.ExtensionMethods
{
    public static class ObservableCollectionExtensionMethods
    {
        public static void ReplaceObservableCollectionItems<T>(this IList<T> list, ObservableCollection<T> oc)
        {
            oc.Clear();
            foreach (var item in list)
            {
                oc.Add(item);
            }
        }
    }
}
