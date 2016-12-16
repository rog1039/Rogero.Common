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

        public static bool Search(PropertyInfo[] properties, object obj, string searchText)
        {
            return Matches(properties, obj, searchText, 0);
        }

        protected static bool Matches<T>(PropertyInfo[] properties, T item, string searchText, int depth = 0)
        {
            if (string.IsNullOrWhiteSpace(searchText)) return true;
            if (depth > 3) return false;

            foreach (var propertyInfo in properties)
            {
                var type = propertyInfo.PropertyType;
                if (type == typeof(string))
                {
                    var value = (string)propertyInfo.GetValue(item);
                    var result = value != null && value.InsensitiveContains(searchText);
                    if (result) return true;
                }
                if (type == typeof(int))
                {
                    var value = ((int)propertyInfo.GetValue(item));
                    var result = value.ToString().InsensitiveContains(searchText);
                    if (result) return true;
                }
                if (type == typeof(decimal))
                {
                    var value = ((decimal)propertyInfo.GetValue(item));
                    var result = value.ToString().InsensitiveContains(searchText);
                    if (result) return true;
                }
                if (type == typeof(double))
                {
                    var value = ((double)propertyInfo.GetValue(item));
                    var result = value.ToString().InsensitiveContains(searchText);
                    if (result) return true;
                }
                //if (!IsSimple(type))
                //{
                //    var obj = propertyInfo.GetValue(item);
                //    if (obj != null)
                //    {
                //        var result = Matches(obj, searchText, depth + 1);
                //        if (result) return true;
                //    }
                //}
            }
            return false;
        }
    }
}
