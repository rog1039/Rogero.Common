using System.Threading.Tasks;

namespace Rogero.Common.ExtensionMethods
{
    public static class TaskExtensionMethods
    {
        public static Task<T> ToTask<T>(this T obj) => Task.FromResult(obj);
    }
}