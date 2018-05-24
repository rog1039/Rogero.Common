﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Rogero.Options;

namespace Rogero.Common.ExtensionMethods
{
    public static class ObjectExtensions
    {
        public static Option<T> FirstNotNull<T>(this T obj, params T[] others)
        {
            if (obj != null) return obj;
            foreach (var other in others)
            {
                if (other != null) return other;
            }
            return Option<T>.None;
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
