using System.Collections;

namespace Rogero.Common;

public static class WraparoundIndexCalc
{
    private static int PreviousIndex(int listSize, int currentIndex)
    {
        return currentIndex <= 0
            ? GetFinalIndex(listSize)
            : currentIndex - 1;
    }

    private static int NextIndex(int listSize, int currentIndex)
    {
        return currentIndex >= GetFinalIndex(listSize)
            ? 0
            : currentIndex + 1;
    }

    private static int GetFinalIndex(int listSize)
    {
        return listSize - 1;
    }

    public static int PreviousIndex<T>(this IList<T> list, int currentIndex)
    {
        return PreviousIndex(list.Count, currentIndex);
    }

    public static int NextIndex<T>(this IList<T> list, int currentIndex)
    {
        return NextIndex(list.Count, currentIndex);
    }
    
    public static T PreviousItem<T>(this IList<T> list, int index) => list[list.PreviousIndex(index)];
    public static T NextItem<T>(this IList<T> list, int index) => list[list.NextIndex(index)];
    
    public static T PreviousItem<T>(this IList<T> list, T item)
    {
        var currentIndex = list.IndexOf(item);
        return list.PreviousItem(currentIndex);
    }
    public static T NextItem<T>(this IList<T> list, T item)
    {
        var currentIndex = list.IndexOf(item);
        return list.NextItem(currentIndex);
    }
}