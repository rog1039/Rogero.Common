namespace Rogero.Common.ExtensionMethods;

public static class TaskExtensionMethods
{
    public static Task<T> ToTask<T>(this T obj) => Task.FromResult(obj);
        
    public static Task<T> ToTaskIfNotTask<T>(this T obj)
    {
        if (obj is Task<T> t) return t;
            
        return Task.FromResult(obj);
    }
        
}