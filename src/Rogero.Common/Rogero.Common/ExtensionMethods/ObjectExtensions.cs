#nullable enable
using Optional;

namespace Rogero.Common.ExtensionMethods;

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

    public static TReturn ObjNullMap<T, TReturn>(this T obj, Func<T, TReturn> func)
        where T : class
        where TReturn : class
    {
        if (obj != null) return func(obj);
        return null;
    }

    public static TReturn ObjNullMap<T, TReturn>(this T? obj, Func<T, TReturn> some, Func<TReturn> none)
    {
        return obj is not null 
            ? some(obj) 
            : none();
    }
        
#nullable enable
    public static void ObjNullMatch<T>(this T obj, Action<T> some)
    {
        if (obj is not null)
        {
            some(obj!);
        }
    }
    public static void ObjNullMatch<T>(this T obj, Action<T> some, Action none)
    {
        if (obj is not null)
        {
            some(obj!);
        }
        else
        {
            none();
        }
    }
#nullable disable

    public static ObjectTaskAnalysis DetermineTaskTypeFromObject(this object obj)
    {
        var type = obj.GetType();
        return DetermineTaskTypeFromType(type);
    }

    public static ObjectTaskAnalysis DetermineTaskTypeFromType(this Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            return ObjectTaskAnalysis.TaskOfT;
        }

        if (type == typeof(Task))
        {
            return ObjectTaskAnalysis.Task;
        }

        return ObjectTaskAnalysis.Neither;
    }

    public static Task<T> AsTask<T>(this object obj)
    {
        if (obj is Task<T> task)
            return task;
        else
            return Task.FromResult((T) obj);
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


#nullable  enable
    public static T? TryCast<T>(this object o) where T: class
    {
        if (o is T t) return t;
        return null;
    }
#nullable disable
}
    
public enum ObjectTaskAnalysis{ Neither, Task, TaskOfT}