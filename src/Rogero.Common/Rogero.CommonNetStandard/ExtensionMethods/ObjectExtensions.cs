using System;
using Optional;

namespace Rogero.Common.ExtensionMethods
{
    public static class ObjectExtensions
    {
        public static Option<T> FirstNotNull<T>(this T obj, params T[] others)
        {
            if (obj != null) return Option.Some(obj);
            foreach (var other in others)
            {
                if (other != null) return Option.Some(other);
            }

            return Option.None<T>();
        }

        /// <summary>
        /// Returns true if the type is a reference type that is not a string.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsReference(this Type type)
        {
            if (type == typeof(string)) return false;
            if (type.IsPrimitive) return false;

            return true;
        }

        public static bool IsNotNull(this object o)
        {
            return !(o is null);
        }


//        public static bool IsPrimitive(this Type type)
//        {
//            if (type == typeof(String)) return true;
//            return (type.IsValueType & type.IsPrimitive);
//        }

        //public static T Combine<T>(params object[] objects)
        //{
        //    var instance = Activator.CreateInstance(typeof(T));
        //    var writableProps = typeof(T).GetProperties().Where(z => z.CanWrite).ToList();
        //    var sourceProps = objects.ToDictionary(z => z.GetType(),
        //                                               z => z.GetType().GetProperties().Where(x => x.CanRead));
        //    foreach (var writableProp in writableProps)
        //    {
        //        var value = GetValue(writableProp, sourceProps, objects);
        //    }
        //}

        //private static object GetValue(PropertyInfo writableProp, Dictionary<Type, IList<PropertyInfo>> sourceProps, object[] objects)
        //{
        //    foreach (var obj in objects)
        //    {
        //        var props = sourceProps[obj.GetType()].Where(z => z.Name == writableProp.Name);

        //    }
        //}
    }
}
