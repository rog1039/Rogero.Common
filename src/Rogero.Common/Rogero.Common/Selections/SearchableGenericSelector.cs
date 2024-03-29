using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Reflection;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Rogero.Common.ExtensionMethods;
using Rogero.Common.Infrastructure;


namespace Rogero.Common.Selections;

public class SearchableGenericSelector<T> : GenericSelector<T>
{
    public ReactiveProperty<string> SearchText { get; } = new ReactiveProperty<string>();
    /// <summary>
    /// The ObservableCollection containing the items after the search is performed.
    /// </summary>
    public ObservableCollection<T> ItemSource { get; } = new ObservableCollection<T>();

    private readonly TimeSpan       _searchThrottleDelay;
    private readonly PropertyInfo[] _itemTypeProperties = typeof(T).GetProperties();

    public SearchableGenericSelector(TimeSpan searchThrottleDelay = default(TimeSpan))
    {
        _searchThrottleDelay = searchThrottleDelay == default(TimeSpan)
            ? TimeSpan.FromMilliseconds(250)
            : searchThrottleDelay;
        SearchText
            .Throttle(_searchThrottleDelay)
            .ObserveOnUIDispatcher()
            .Subscribe(SearchTextChanged);

        //Listen for changes on the Items collection and update ItemsSource as needed.
        var ItemsChangedObservable =
            Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                eh => Items.CollectionChanged += eh,
                eh => Items.CollectionChanged -= eh);

        ItemsChangedObservable
            .Throttle(TimeSpan.FromMilliseconds(200))
            .ObserveOnUIDispatcher()
            .Subscribe(z => SearchTextChanged(SearchText.Value));
    }

    public void AddNewItem(T item)
    {
        Items.Add(item);
        ItemSource.Add(item);
    }

    protected virtual void SearchTextChanged(string searchText)
    {
        var selected = SelectedItem.Value;

        var matchingItems = GetSearchResults(searchText);

        matchingItems.ReplaceObservableCollectionItems(ItemSource);
        var newSelected = Items.FirstOrDefault(z => z.Equals(selected));
        SelectedItem.Value = newSelected;
    }

    protected virtual IEnumerable<T> GetSearchResults(string searchText)
    {
        var matchingItems = ObjectTextSearcher.FindMatches(Items, searchText);
        return matchingItems;
    }

    public override void ReplaceItemSource(IEnumerable<T> records)
    {
        records.ReplaceObservableCollectionItems(Items);
        SearchTextChanged(SearchText.Value);
    }

    public override void ClearItems()
    {
        base.ClearItems();
        SearchTextChanged(SearchText.Value);
    }
}