namespace Rogero.Common.ExtensionMethods;

#nullable enable

public static class NullableExtensions
{
    public static T ValueOrThis<T>(this T? val, T other) where T : struct
    {
       return val.HasValue 
          ? val.Value 
          : other;
    }
}