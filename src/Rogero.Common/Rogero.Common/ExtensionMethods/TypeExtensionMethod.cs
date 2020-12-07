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
    }

    public static class TaskExtensionMethods
    {
        public static Task<T> ToTask<T>(this T obj) => Task.FromResult(obj);
    }
}
