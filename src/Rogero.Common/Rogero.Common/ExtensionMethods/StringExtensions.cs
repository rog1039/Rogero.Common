using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(input, part, CompareOptions.IgnoreCase) >= 0;
        }

        public static string RemoveAllWhitespace(this string s)
        {
            var result = s.Replace("\r\n", "");
            result = result.Replace("\r", "");
            result = result.Replace("\n", "");
            result = result.Replace("\t", "");
            
            return result;
        }
    }
}
