using System.Collections.ObjectModel;
using Reactive.Bindings;
using Rogero.Common.ExtensionMethods;


namespace Rogero.Common.Selections;

public class GenericSelector<T>
{
    public ReactiveProperty<T>     SelectedItem { get; } = new ReactiveProperty<T>(default(T));
    public ObservableCollection<T> Items        { get; } = new ObservableCollection<T>();

    public virtual void ReplaceItemSource(IEnumerable<T> records)
    {
        try
        {
            var selected = SelectedItem.Value;
            Items.Clear();
            Items.AddRange(records);
            var newSelected = records.FirstOrDefault(z => z.Equals(selected));
            SelectedItem.Value = newSelected;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public virtual void ClearItems()
    {
        Items.Clear();
    }
}

public static class GenericSelectorExtensions
{
    public static void SelectFirst<T>(this GenericSelector<T> selector)
    {
        selector.SelectedItem.Value = selector.Items.FirstOrDefault();
    }
        
    public static void SelectLast<T>(this GenericSelector<T> selector)
    {
        selector.SelectedItem.Value = selector.Items.LastOrDefault();}
}

public class GenericMultiSelector<T>
{
    public ObservableCollection<T> SelectedItems { get; } = new ObservableCollection<T>();
    public ObservableCollection<T> Items         { get; } = new ObservableCollection<T>();

    public virtual void ReplaceItemSource(IEnumerable<T> records)
    {
        try
        {
            var selectedItems = SelectedItems.ToList();
            Items.Clear();
            Items.AddRange(records);
            foreach (var selectedItem in selectedItems)
            {
                var newItem = Items.First(z => selectedItem.Equals(z));
                SelectedItems.Add(newItem);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}