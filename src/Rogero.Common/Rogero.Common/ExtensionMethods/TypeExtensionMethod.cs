using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rogero.Common.ExtensionMethods
{
    public static class TypeExtensionMethod
    {
        public static bool ImplementsInterface<T>(this Type t)
        {
            if (t == null) return false;
            
            var interfaces = t.GetInterfaces();
            var implementsInterface = interfaces.Any(i => i.AssemblyQualifiedName == typeof(T).AssemblyQualifiedName);
            return implementsInterface;
        }

        public static void Test()
        {
            var basetype = typeof(object).BaseType;
            var f1 = typeof(TypeExtensionMethod).ImplementsInterface<ICloneable>();
        }
        
        public static bool IsNullable(this Type type)
        {
            var isNullable = Nullable.GetUnderlyingType(type) != null;
            if (isNullable) return true;

            //Check nullable attribute: https://github.com/dotnet/roslyn/blob/master/docs/features/nullable-metadata.md
            return HasNullableAttribute(type);
        }

        public static bool HasNullableAttribute(this Type type)
        {
            var attribute = type.GetAttributeSingleOrDefault("NullableAttribute", true);
            return attribute != null;
        }
        
        public static object GetAttributeSingleOrDefault(this Type type, string attributeName,
                                                         bool      inheritBaseAttributes = false)
        {
            var attribute = type
                .GetCustomAttributes(inheritBaseAttributes)
                .SingleOrDefault(z => z.GetType().Name == attributeName);
            return attribute;
        }
    }

    public static class TaskExtensionMethods
    {
        public static Task<T> ToTask<T>(this T obj) => Task.FromResult(obj);
    }
}
