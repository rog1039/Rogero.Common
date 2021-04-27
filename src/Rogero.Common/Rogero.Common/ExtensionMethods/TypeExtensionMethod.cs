using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

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
        
        public static object GetAttributeSingleOrDefault(this Type type, string attributeName,
                                                         bool      inheritBaseAttributes = false)
        {
            var attribute = type
                .GetCustomAttributes(inheritBaseAttributes)
                .SingleOrDefault(z => z.GetType().Name == attributeName);
            return attribute;
        }

        public static object GetAttributeSingleOrDefault(this MemberInfo memberInfo, string attributeName,
                                                         bool            inheritBaseAttributes = false)
        {
            var attribute = memberInfo
                .GetCustomAttributes(inheritBaseAttributes)
                .SingleOrDefault(z => z.GetType().Name == attributeName);
            return attribute;
        }

        public static object GetAttributeSingleOrDefault(this ParameterInfo parameterfInfo, string attributeName,
                                                         bool            inheritBaseAttributes = false)
        {
            var attribute = parameterfInfo
                .GetCustomAttributes(inheritBaseAttributes)
                .SingleOrDefault(z => z.GetType().Name == attributeName);
            return attribute;
        }

        public static T GetAttributeSingleOrDefault<T>(this Type type, bool inheritBaseAttributes = false)
        {
            var attribute = type
                .GetCustomAttributes(inheritBaseAttributes)
                .SingleOrDefault(z => z.GetType() == typeof(T));
            return (T) attribute;
        }

        public static T GetAttributeSingle<T>(this Type type, bool inheritBaseAttributes = false)
        {
            var attribute = type
                .GetCustomAttributes(inheritBaseAttributes)
                .Single(z => z.GetType() == typeof(T));
            return (T) attribute;
        }
        
        public static T GetSingleAttributeIncludingFromInterfaces<T>(this Type type)
        {
            var typeAttributes = type
                .GetCustomAttributes(true)
                .WhereCastTo<T>();
            
            var interfaces     = type.GetInterfaces();
            var attributes = interfaces
                .SelectMany(z => z.GetCustomAttributes(true))
                .WhereCastTo<T>()
                .Concat(typeAttributes);

            return attributes.Single();
        }

        public static IList<Type> GetSelfAndInnerTypes(this Type type)
        {
            var types = new List<Type>();

            var queue = new Queue<Type>(){};
            queue.Enqueue(type);
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                types.Add(item);
                item.GenericTypeArguments
                    .MyForEach(z => queue.Enqueue(z));
            }

            return types.Distinct().ToList();
        }
        
        public static string ToClosedTypeName(this Type type)
        {
            if (!type.IsGenericType) return type.Name;

            var genericTypes = type
                    .GetGenericArguments()
                    .Select(ga => ga.ToClosedTypeName())
                ;
            var genericText    = string.Join(", ", genericTypes);
            var argCountLength = type.GetGenericArguments().Length.ToString().Length;
            var suffixLength   = 1 + argCountLength; // ex: `1, `3, `17
            var nonSuffixedTypeName = type
                .GetGenericTypeDefinition()
                .Name[..^suffixLength];
            
            return $"{nonSuffixedTypeName}<{genericText}>";
        }

        public static bool IsClosedGenericOf(this Type type, Type genericType)
        {
            if (type.GenericTypeArguments.Length == 0) return false;
            return type.GetGenericTypeDefinition() == genericType;
        }

        static TypeExtensionMethod()
        {
        }
    }
}
