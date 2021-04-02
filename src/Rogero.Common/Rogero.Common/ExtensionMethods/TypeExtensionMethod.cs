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
            return HasNullableAttributeSetTo2(type);
        }

        /// <summary>
        /// See https://codeblog.jonskeet.uk/2019/02/10/nullableattribute-and-c-8/ for an explanation.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasNullableAttributeSetTo2(this Type type)
        {
            var nullableAttribute = type.GetAttributeSingleOrDefault("NullableAttribute", false);

            var isNullable = nullableAttribute
                .ObjNullMap(attribute => attribute.GetType().GetProperty("NullableFlags"))
                .ObjNullMap(flagsProp => flagsProp.GetValue(nullableAttribute) as byte[])
                .ObjNullMatch(
                    (bytes => bytes[0] == 2 ? true : false),
                    () => false);
        
     
        
            /*
             * Old way this method worked -- should be functionally equivalent to the above code but written in the traditional imperative style.
             */
            var nullableAttribute2      = type.GetAttributeSingleOrDefault("NullableAttribute", true);
            if (nullableAttribute2 is null) return false;
            
            var flagsAttribute = nullableAttribute2.GetType().GetProperty("NullableFlags");
            if (flagsAttribute is null) return false;

            var flagsValue = flagsAttribute.GetValue(nullableAttribute2);
            if (flagsValue is null) return false;

            return flagsValue switch
            {
                byte[] bytes when bytes[0] == 2 => true,
                _                               => false
            };
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
