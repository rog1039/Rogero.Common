using System;

public static class ArgumentValidationExtensions
{
    public static T ThrowIfNull<T>(this T o, string paramName) where T : class
    {
        if (o == null)
            throw new ArgumentNullException(paramName);

        return o;
    }

    public static string ThrowIfNullOrWhitespace(this string s, string paramName) 
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentNullException(paramName);

        return s;
    }
}