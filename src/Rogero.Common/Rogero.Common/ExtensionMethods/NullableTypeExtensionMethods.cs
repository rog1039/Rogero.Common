using System;
using System.Linq;
using System.Reflection;

namespace Rogero.Common.ExtensionMethods
{
    public static class NullableTypeExtensionMethods
    {
        private static BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        //Check nullable attribute: https://github.com/dotnet/roslyn/blob/master/docs/features/nullable-metadata.md
        //Jon Skeet: https://codeblog.jonskeet.uk/2019/02/10/nullableattribute-and-c-8/
        private static readonly int MagicNullableByteValue = 2;

        public static bool ReturnIsNullable(this MethodInfo methodInfo)
        {
            var nullableContextAttribute = methodInfo
                .CustomAttributes
                .SingleOrDefault(z => z.AttributeType.Name.Equals("NullableContextAttribute"));
            
            if (nullableContextAttribute is null || nullableContextAttribute.ConstructorArguments.Count == 0)
            {
                return false;
            }

            var firstTypedArg = nullableContextAttribute.ConstructorArguments[0];
            
            return firstTypedArg.Value is byte b && b == MagicNullableByteValue;
        }
        
        public static byte[] GetNullableAttributeByteArray(this ParameterInfo propertyInfo)
        {
            var nullableAttribute = propertyInfo.GetAttributeSingleOrDefault("NullableAttribute");
            var isNullable        = GetNullableByteArrayFromNullableAttribute(nullableAttribute);
            return isNullable;
        }

        public static byte[] GetNullableAttributeByteArray(this MemberInfo propertyInfo)
        {
            var nullableAttribute = propertyInfo.GetAttributeSingleOrDefault("NullableAttribute");
            var isNullable        = GetNullableByteArrayFromNullableAttribute(nullableAttribute);
            return isNullable;
        }

        public static byte[] GetNullableByteArrayFromNullableAttribute(object nullableAttribute)
        {
            var isNullable = nullableAttribute
                .ObjNullMap(attribute => attribute.GetType().GetField("NullableFlags", BindingFlags))
                .ObjNullMap(flagsProp => flagsProp.GetValue(nullableAttribute) as byte[])
                .ObjNullMatch(
                    bytes => bytes,
                    () => new byte[0]);
            return isNullable;
        }

        public static bool IsNullable(this MemberInfo propertyInfo)
        {
            var isBuiltInNullable = IsBuiltInNullableType(propertyInfo);
            if (isBuiltInNullable) return true;
            var nullableAttribute = propertyInfo.GetAttributeSingleOrDefault("NullableAttribute");
            return DoesNullableAttributeHaveMagicNullableByte(nullableAttribute);
        }

        private static bool IsBuiltInNullableType(MemberInfo memberInfo)
        {
            return memberInfo switch
            {
                FieldInfo fieldInfo        => fieldInfo.FieldType.IsNullable(),
                MethodInfo methodInfo      => methodInfo.ReturnType.IsNullable(),
                PropertyInfo propertyInfo1 => propertyInfo1.PropertyType.IsNullable(),
                _                          => throw new ArgumentOutOfRangeException(nameof(memberInfo))
            };
        }

        public static bool IsNullable(this Type type)
        {
            var isNullableType = Nullable.GetUnderlyingType(type) != null;
            return isNullableType || type.HasNullableAttributeSetTo2();
        }
        
        /// <summary>
        /// See https://codeblog.jonskeet.uk/2019/02/10/nullableattribute-and-c-8/ for an explanation.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasNullableAttributeSetTo2(this Type type)
        {
            var nullableAttribute = type.GetAttributeSingleOrDefault("NullableAttribute", false);
            return DoesNullableAttributeHaveMagicNullableByte(nullableAttribute);


            /*
             * Old way this method worked -- should be functionally equivalent to the above code but written in the traditional imperative style.
             */
            var nullableAttribute2 = type.GetAttributeSingleOrDefault("NullableAttribute", true);
            if (nullableAttribute2 is null) return false;
            
            var flagsAttribute = nullableAttribute2.GetType().GetProperty("NullableFlags", BindingFlags.Instance 
                                                                        | BindingFlags.Public | BindingFlags.NonPublic);
            if (flagsAttribute is null) return false;

            var flagsValue = flagsAttribute.GetValue(nullableAttribute2);
            if (flagsValue is null) return false;

            return flagsValue switch
            {
                byte[] bytes when bytes[0] == 2 => true,
                _                               => false
            };
        }

        private static bool DoesNullableAttributeHaveMagicNullableByte(object nullableAttribute)
        {
            var isNullable = nullableAttribute
                .ObjNullMap(attribute => attribute.GetType().GetField("NullableFlags", BindingFlags))
                .ObjNullMap(flagsField => flagsField.GetValue(nullableAttribute) as byte[])
                .ObjNullMatch(
                    (bytes => bytes[0] == MagicNullableByteValue),
                    () => false);

            return isNullable;
        }
    }
}