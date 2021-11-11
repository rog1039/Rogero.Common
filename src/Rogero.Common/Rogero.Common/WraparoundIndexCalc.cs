using System.Collections;

namespace Rogero.Common;

public static class WraparoundIndexCalc
{
    public static int PreviousIndex(this IList list, int currentIndex)
    {
        return PreviousIndex(list.Count, currentIndex);
    }

    public static int NextIndex(this IList list, int currentIndex)
    {
        return NextIndex(list.Count, currentIndex);
    }

    public static int PreviousIndex(int listSize, int currentIndex)
    {
        return currentIndex <= 0
            ? GetFinalIndex(listSize)
            : currentIndex - 1;
    }

    public static int NextIndex(int listSize, int currentIndex)
    {
        return currentIndex >= GetFinalIndex(listSize)
            ? 0
            : currentIndex + 1;
    }

    public static int GetFinalIndex(int listSize)
    {
        return listSize - 1;
    }
}