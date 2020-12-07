using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Rogero.Common.ExtensionMethods
{
    public static class StringExtensions
    {
        public static string Bracketize(this string input)
        {
            return "[" + input + "]";
        }

        public static string Bracify(this string input)
        {
            return "{" + input + "}";
        }

        public static string Repeat(this string input, int repeatCount)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < repeatCount; i++)
            {
                sb.Append(input);
            }
            return sb.ToString();
        }
        public static bool InsensitiveEquals(this string s1, string s2)
        {
            var oneIsNullOtherIsnt = (s1 == null && s2 != null) || (s1 != null && s2 == null);
            if (oneIsNullOtherIsnt)
                return false;

            var bothAreNull = s1 == null && s2 == null;
            if (bothAreNull)
                return true;

            return s1.Length == s2.Length && s1.IndexOf(s2, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public static bool InsensitiveContains(this string input, string part)
        {
            if (part == null || input == null) return false;
            return input.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0;
            //return CultureInfo.InvariantCulture.CompareInfo.IndexOf(input, part, CompareOptions.IgnoreCase) >= 0;
        }

        public static string RemoveAllWhitespace(this string s)
        {
            var result = s.Replace("\r\n", "");
            result = result.Replace("\r", "");
            result = result.Replace("\n", "");
            result = result.Replace("\t", "");
            
            return result;
        }

        public static string RemoveLeft(this string s, int numberOfCharacters)
        {
            if(numberOfCharacters > s.Length) throw new ArgumentException("Number of characters to remove cannot be larger than length of string");
            return s.Substring(numberOfCharacters, s.Length - numberOfCharacters);
        }

        public static void ToConsoleWriteLine(this string s)
        {
            Console.WriteLine(s);
        }

        public static void ToDebugWriteLine(this string s)
        {
            Debug.WriteLine(s);
        }

        public static bool IsNullOrWhitespace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        public static bool IsNotNullOrWhitespace(this string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }

        public static string Left(this string s, int count)
        {
            var charCount = Math.Min(s.Length, count);
            return s.Substring(0, charCount);
        }

        public static string Right(this string s, int count)
        {
            var charCount = Math.Min(s.Length, count);
            return s.Substring(s.Length - charCount, charCount);
        }

        public static string StringJoin(this IEnumerable<string> list, string separator)
        {
            return string.Join(separator, list);
        }

        public static string EscapeString(this string s)
        {
            return s
                .Replace("\\", "\\\\")  //escape backslashes
                .Replace("\"", "\\\""); //escape quotations;
        }
    }

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
}
