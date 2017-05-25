using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Rogero.Common.ExtensionMethods;
using Rogero.ReactiveProperty;

namespace Rogero.Common.Selections
{
    public class GenericSelector<T>
    {
        public ReactiveProperty<T> SelectedItem { get; } = new ReactiveProperty<T>();
        public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();

        public virtual void ReplaceItemSource(IList<T> records)
        {
            var selected = SelectedItem.Value;
            Items.Clear();
            Items.AddRange(records);
            var newSelected = records.FirstOrDefault(z => z.Equals(selected));
            SelectedItem.Value = newSelected;
        }
    }
}