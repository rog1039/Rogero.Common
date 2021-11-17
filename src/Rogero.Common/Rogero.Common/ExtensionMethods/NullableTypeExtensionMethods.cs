using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Rogero.Common.ExtensionMethods;

public static class NullableTypeExtensionMethods
{
    private static BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    //Check nullable attribute: https://github.com/dotnet/roslyn/blob/master/docs/features/nullable-metadata.md
    //Jon Skeet: https://codeblog.jonskeet.uk/2019/02/10/nullableattribute-and-c-8/
    private static readonly int MagicNonNullableByteValue = 1;
    private static readonly int MagicNullableByteValue    = 2;

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

    public static Type GetMemberType(MemberInfo memberInfo)
    {
        switch (memberInfo)
        {
            case ComAwareEventInfo comAwareEventInfo:
                break;
            case EventInfo eventInfo:
                break;
            case ConstructorBuilder constructorBuilder:
                break;
            case DynamicMethod dynamicMethod:
                break;
            case EnumBuilder enumBuilder:
                break;
            case FieldBuilder fieldBuilder:
                break;
            case GenericTypeParameterBuilder genericTypeParameterBuilder:
                break;
            case MethodBuilder methodBuilder:
                break;
            case PropertyBuilder propertyBuilder:
                break;
            case TypeBuilder typeBuilder:
                break;
            case ConstructorInfo constructorInfo:
                break;
            case FieldInfo fieldInfo:
                return fieldInfo.FieldType;
                break;
            case MethodInfo methodInfo:
                break;
            case MethodBase methodBase:
                break;
            case PropertyInfo propertyInfo:
                return propertyInfo.PropertyType;
                break;
            case TypeDelegator typeDelegator:
                break;
            case TypeInfo typeInfo:
                break;
            case Type type:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(memberInfo));
        }

        throw new NotImplementedException();
    }

    public static bool IsMemberNullable(this MemberInfo memberInfo)
    {
        var memberType  = GetMemberType(memberInfo);
        var isValueType = memberType.IsValueType;
        if (isValueType)
        {
            /*
             * If the type is a value type, and it is not wrapped in a nullable type, then it must be
             * non-nullable. The nullable attribute code below only applies to non-value type types.
             */
            var isBuiltInNullable = IsNullableOfT(memberInfo);
            return isBuiltInNullable;
        }

        /*
         * Check the metadata of a reference type via attributes.
         * There is a field/property level NullableAttribute and an owning-type NullableContextAttribute.
         * NullableAttribute takes precedence and in it's absence we look for a NullableContextAttribute.
         * If both are missing then I think the code is null-oblivious.
         */

        var nullableAttribute      = memberInfo.GetAttributeSingleOrDefault("NullableAttribute");
        var nullableAttributeValue = GetNullableAttributeValue(nullableAttribute);

        switch (nullableAttributeValue)
        {
            case 0:
                //Fall through since 0 means nothing, still need to check for NullableContextAttribute
                break;
            case 1:
                return false;
            case 2:
                return true;
        }

        /*
         * Now, we may need to examine the declaring type for a NullableContextAttribute. However, it should be noted
         * that NullableContextAttribute only applies to fields that are **not** reference types.
         * See:
         * https://github.com/dotnet/roslyn/blob/main/docs/features/nullable-metadata.md
         *
         * Also, might be worth reading:
         * https://github.com/OData/ModelBuilder/issues/16
         */

        var declaringType            = memberInfo.DeclaringType;
        var nullableContextAttribute = declaringType.GetAttributeSingleOrDefault("NullableContextAttribute");
        var nullableContextValue     = GetNullableContextAttributeValue(nullableContextAttribute);

        return nullableContextValue switch
        {
            //A 0 means the type is nullable oblivious and is thus not nullable.
            0 => false,
            //1 is not-nullable
            1 => false,
            //2 is nullable
            2 => true,
            //Should never get here... since the 0,1,2 should be all possible cases above.
            _ => throw new ArgumentException(nameof(nullableContextValue), $"Expected 0, 1, or 2; {nullableContextValue} was unexpected."),
        };
    }

    public static bool IsNullable2(this MemberInfo propertyInfo)
    {
        var isBuiltInNullable = IsNullableOfT(propertyInfo);
        if (isBuiltInNullable) return true;

        var nullableAttribute    = propertyInfo.GetAttributeSingleOrDefault("NullableAttribute");
        var hasMagicNullableByte = DoesNullableAttributeHaveMagicNullableByte(nullableAttribute);

        if (hasMagicNullableByte) return true;

        var declaringTypeHasNullableContextAttribute = propertyInfo
            .DeclaringType
            .GetAttributeSingleOrDefault("NullableContextAttribute");
        if (declaringTypeHasNullableContextAttribute is null) throw new NotImplementedException();

        return false;
    }

    /// <summary>
    /// Is the member have the type: Nullable<T>?
    /// </summary>
    /// <param name="memberInfo"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static bool IsNullableOfT(MemberInfo memberInfo)
    {
        return memberInfo switch
        {
            FieldInfo fieldInfo        => fieldInfo.FieldType.IsNullableOfT(),
            MethodInfo methodInfo      => methodInfo.ReturnType.IsNullableOfT(),
            PropertyInfo propertyInfo1 => propertyInfo1.PropertyType.IsNullableOfT(),
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

    /// <summary>
    /// Returns 0 if the Flag field is not present, otherwise, returns the value.
    /// </summary>
    /// <param name="nullableAttribute"></param>
    /// <returns></returns>
    private static int GetNullableContextAttributeValue(object nullableAttribute)
    {
        var nullableByteValue = nullableAttribute
            .ObjNullMap(attribute => attribute.GetType().GetField("Flag", BindingFlags))
            .ObjNullMap(flagsField => (object) (byte) flagsField.GetValue(nullableAttribute))
            .ObjNullMatch(
                (byteValue => (byte) byteValue),
                () => (byte) 0);

        return nullableByteValue;
    }

    /// <summary>
    /// Returns 0 if the nullable attribute is not present, otherwise, returns the value of the nullable attribute.
    /// NullableFlags contains a byte[].
    /// </summary>
    /// <param name="nullableAttribute"></param>
    /// <returns></returns>
    private static int GetNullableAttributeValue(object nullableAttribute)
    {
        var nullableByteValue = nullableAttribute
            .ObjNullMap(attribute => attribute.GetType().GetField("NullableFlags", BindingFlags))
            .ObjNullMap(flagsField => flagsField.GetValue(nullableAttribute) as byte[])
            .ObjNullMatch(
                (bytes => bytes[0]),
                () => 0);

        return nullableByteValue;
    }

    private static bool DoesNullableAttributeHaveMagicNullableByte(object nullableAttribute)
    {
        var nullableAttributeValue = GetNullableAttributeValue(nullableAttribute);
        return nullableAttributeValue == MagicNullableByteValue;
    }

    /// <summary>
    /// If the type is Nullable<T>, return T, otherwise return the type itself.
    /// Unwraps a Nullable<T> if you will.
    /// </summary>
    /// <param name="propertyType"></param>
    /// <returns></returns>
    public static Type UnwrapNullable(this Type propertyType)
    {
        return Nullable.GetUnderlyingType(propertyType) is { } type 
            ? type 
            : propertyType;
    }
    
    public static bool IsNullableOfT(this Type type)
    {
        var isNullable = Nullable.GetUnderlyingType(type) != null;
        return isNullable;
    }
}