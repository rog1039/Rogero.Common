namespace Rogero.Common.ExtensionMethods;

public static class NullableExtensions
{
    public static T ValueOrThis<T>(this T? val, T other) where T : struct
    {
        if (val.HasValue) return val.Value;
        return other;
    }
}