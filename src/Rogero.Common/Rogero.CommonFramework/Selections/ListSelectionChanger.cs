
using System;
using System.Collections.ObjectModel;
using Reactive.Bindings;

namespace Rogero.Common.Selections
{
    public static class ListSelectionChanger
    {
        private const int StartingIndex = 0;

        public static void MoveUp<T>(ObservableCollection<T> collection, ReactiveProperty<T> current)
        {
            var index = collection.IndexOf(current.Value);
            if (index == -1 || index == 0)
            {
                MoveToEnd(collection, current);
            }
            else
            {
                index--;
                MoveTo(collection, current, index);
            }
        }

        public static void MoveToEnd<T>(ObservableCollection<T> collection, ReactiveProperty<T> current)
        {
            MoveTo(collection, current, FinalIndex(collection));
        }

        private static int FinalIndex<T>(ObservableCollection<T> collection)
        {
            return collection.Count - 1;
        }

        public static void MoveDown<T>(ObservableCollection<T> collection, ReactiveProperty<T> current)
        {
            var index = collection.IndexOf(current.Value);
            if (index == -1 || index == FinalIndex(collection))
            {
                MoveToStart(collection, current);
            }
            else
            {
                index++;
                MoveTo(collection, current, index);
            }
        }

        public static void MoveToStart<T>(ObservableCollection<T> collection, ReactiveProperty<T> current)
        {
            MoveTo(collection, current, StartingIndex);
        }

        public static void MoveTo<T>(ObservableCollection<T> collection, ReactiveProperty<T> current, int index)
        {
            CheckIndex(collection, index);
            current.Value = collection[index];
        }

        private static void CheckIndex<T>(ObservableCollection<T> collection, int index)
        {
            if (index < 0 || index >= collection.Count)
                throw new ArgumentException(
                    $"Index provided, {index}, was invalid");
        }
    }

    public static class ListSelectionChangerByRef
    {
        private const int StartingIndex = 0;

        public static void MoveUp<T>(ObservableCollection<T> collection, ref T current)
        {
            var index = collection.IndexOf(current);
            if (index == -1 || index == 0)
            {
                MoveToEnd(collection, ref current);
            }
            else
            {
                index--;
                MoveTo(collection, ref current, index);
            }
        }

        public static void MoveToEnd<T>(ObservableCollection<T> collection, ref T current)
        {
            MoveTo(collection, ref current, FinalIndex(collection));
        }

        private static int FinalIndex<T>(ObservableCollection<T> collection)
        {
            return collection.Count - 1;
        }

        public static void MoveDown<T>(ObservableCollection<T> collection, ref T current)
        {
            var index = collection.IndexOf(current);
            if (index == -1 || index == FinalIndex(collection))
            {
                MoveToStart(collection, ref current);
            }
            else
            {
                index++;
                MoveTo(collection, ref current, index);
            }
        }

        public static void MoveToStart<T>(ObservableCollection<T> collection, ref T current)
        {
            MoveTo(collection, ref current, StartingIndex);
        }

        public static void MoveTo<T>(ObservableCollection<T> collection, ref T current, int index)
        {
            CheckIndex(collection, index);
            current = collection[index];
        }

        private static void CheckIndex<T>(ObservableCollection<T> collection, int index)
        {
            if (index < 0 || index >= collection.Count)
                throw new ArgumentException(
                    $"Index provided, {index}, was invalid");
        }
    }
}