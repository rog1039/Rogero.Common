namespace Rogero.Common.ExtensionMethods;

public static class SetExtensionMethods
{
    public static void AddRange<T>(this ISet<T> set, IEnumerable<T> objects)
    {
        foreach (var obj in objects)
        {
            set.Add(obj);
        }
    }

    /// <summary>
    /// Does not modify either set. Returns a new HashSet with the result.
    /// </summary>
    /// <param name="set"></param>
    /// <param name="itemsToRemove"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ISet<T> Subtract<T>(this ISet<T> set, IEnumerable<T> itemsToRemove)
    {
        var newSet = set.MyToHashSet();
        foreach (var item in itemsToRemove)
        {
            newSet.Remove(item);
        }
        return newSet;
    }

    public static ISet<T> ExcludeSet<T>(this ISet<T> set, ISet<T> secondSet)
    {
        var resultSet = new HashSet<T>();
            
        foreach (var item in set)
        {
            if (!secondSet.Contains(item))
                resultSet.Add(item);
        }

        return resultSet;
    }

    public static ISet<T> IntersectSet<T>(this ISet<T> set, ISet<T> secondSet)
    {
        var resultSet = new HashSet<T>();
            
        foreach (var item in set)
        {
            if (secondSet.Contains(item))
                resultSet.Add(item);
        }

        return resultSet;
    }
}