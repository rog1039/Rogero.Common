using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Rogero.Common.ExtensionMethods;
using Rogero.ReactiveProperty;

namespace Rogero.Common.Selections
{
    public class SearchableGenericSelector<T> : GenericSelector<T>
    {
        public ReactiveProperty<string> SearchText { get; } = new ReactiveProperty<string>();
        public ObservableCollection<T> ItemSource { get; } = new ObservableCollection<T>();

        private readonly TimeSpan _searchThrottleDelay;
        private readonly PropertyInfo[] _itemTypeProperties = typeof(T).GetProperties();

        public SearchableGenericSelector(TimeSpan searchThrottleDelay = default(TimeSpan))
        {
            _searchThrottleDelay = searchThrottleDelay == default(TimeSpan)
                ? TimeSpan.FromMilliseconds(250)
                : searchThrottleDelay;
            SearchText
                .Throttle(_searchThrottleDelay)
                .ObserveOnDispatcher()
                .Subscribe(SearchTextChanged);
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
            var matchingItems = ObjectTextSearcher.Search(Items, searchText);
            return matchingItems;
        }

        public override void ReplaceItemSource(IList<T> records)
        {
            records.ReplaceObservableCollectionItems(ItemSource);
            SearchTextChanged(SearchText.Value);
        }
    }
}