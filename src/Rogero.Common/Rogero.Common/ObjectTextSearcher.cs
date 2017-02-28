using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Rogero.Common.ExtensionMethods;

namespace Rogero.Common
{
    public class ObjectTextSearcher
    {
        public static IEnumerable<T> Search<T>(IEnumerable<T> objects, string searchText)
        {
            var itemProps = typeof(T).GetProperties();
            foreach (var obj in objects)
            {
                if (Search(itemProps, obj, searchText)) yield return obj;
            }
        }

        private static bool Search(PropertyInfo[] properties, object obj, string searchText)
        {
            if(string.IsNullOrWhiteSpace(searchText)) return true;
            
            var terms = searchText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var term in terms)
            {
                var match = Matches(properties, obj, term, 0);
                if (!match) return false;
            }
            return true;
        }

        private static bool Matches<T>(PropertyInfo[] properties, T item, string searchText, int depth = 0)
        {
            if (string.IsNullOrWhiteSpace(searchText)) return true;
            if (depth > 3) return false;

            var performingNegationSearch = searchText[0] == '-';
            searchText = performingNegationSearch ? searchText.RemoveLeft(1) : searchText;
            var returnTransform = performingNegationSearch ? (Func<bool, bool>)InvertBoolTransform : DoNothingBoolTransform;

            foreach (var propertyInfo in properties)
            {
                var type = propertyInfo.PropertyType;
                if (type == typeof(string))
                {
                    var value = (string)propertyInfo.GetValue(item);
                    var result = value != null && value.InsensitiveContains(searchText);
                    if (result) return returnTransform(true);
                }
                if (type == typeof(int))
                {
                    var value = ((int)propertyInfo.GetValue(item));
                    var result = value.ToString().InsensitiveContains(searchText);
                    if (result) return returnTransform(true);
                }
                if (type == typeof(decimal))
                {
                    var value = ((decimal)propertyInfo.GetValue(item));
                    var result = value.ToString().InsensitiveContains(searchText);
                    if (result) return returnTransform(true);
                }
                if (type == typeof(double))
                {
                    var value = ((double)propertyInfo.GetValue(item));
                    var result = value.ToString().InsensitiveContains(searchText);
                    if (result) return returnTransform(true);
                }
                if (type == typeof(DateTime))
                {
                    var value = ((DateTime)propertyInfo.GetValue(item));
                    var result = value.ToString("d").InsensitiveContains(searchText);
                    if (result) return returnTransform(true);
                }
            }
            return returnTransform(false);
        }

        private static bool DoNothingBoolTransform(bool b) => b;
        private static bool InvertBoolTransform(bool b) => !b;
    }
}
