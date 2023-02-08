namespace Rogero.Common.ExtensionMethods;

public static class StructExtensions
{
   public static T IfDefaultThen<T>(this T item, T otherValue) where T : struct, IEquatable<T>
   {
      return item.Equals(default) 
         ? otherValue 
         : item;
   }
}