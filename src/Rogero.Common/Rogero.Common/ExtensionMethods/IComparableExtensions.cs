namespace Rogero.Common.ExtensionMethods;

public static class IComparableExtensions
{
   public static (T, T) SortPair<T>(this T t1, T t2) where T: IComparable
   {
      var compareTo = t1.CompareTo(t2);
      return compareTo switch
      {
         < 0 => (t1, t2),
         0 => (t1, t2),
         > 0 => (t2, t1)
      };
   }
   public static List<T> SortMany<T>(this T t1, params T[] t2) where T: IComparable
   {
      List<T> list = [t1, ..t2];
      return list.OrderBy(t => t)
                 .ToList();
   }
}