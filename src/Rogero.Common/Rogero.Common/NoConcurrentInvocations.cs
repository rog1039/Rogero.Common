#nullable enable
using Optional;

namespace Rogero.Common;

public enum NoConcurrentExecutionStatus
{
   Unknown        = 0,
   Ran            = 1,
   AlreadyRunning = 2,
   Error          = 3,
}

/// <summary>
/// A class which makes it impossible for an action to run more than once at the same time. Multiple threads could
/// ask for execution of the action but only one of them will be successful.
/// </summary>
public class NoConcurrentInvocations
{
   private long _isRunning = 0;

   /// <summary>
   /// Ensures that only one call to this method will result in the action being executed. 
   /// </summary>
   /// <param name="action"></param>
   /// <returns></returns>
   public NoConcurrentExecutionStatus Run(Action action)
   {
      if (Interlocked.Exchange(ref _isRunning, 1) == 0)
      {
         try
         {
            action();
            return NoConcurrentExecutionStatus.Ran;
         }
         catch
         {
            return NoConcurrentExecutionStatus.Error;
         }
         finally
         {
            Interlocked.Exchange(ref _isRunning, 0);
         }
      }
      else
      {
         return NoConcurrentExecutionStatus.AlreadyRunning;
      }
   }

   /// <summary>
   /// Ensures that only one call to this method will result in the action being executed. 
   /// </summary>
   /// <param name="action"></param>
   /// <returns></returns>
   public NoConcurrentExecutionResult<T> Run<T>(Func<T> action) where T : class
   {
      if (Interlocked.Exchange(ref _isRunning, 1) == 0)
      {
         try
         {
            var result = action();
            return NoConcurrentExecutionResult<T>.Ran(result);
         }
         catch
         {
            return NoConcurrentExecutionResult<T>.Error();
         }
         finally
         {
            Interlocked.Exchange(ref _isRunning, 0);
         }
      }
      else
      {
         return NoConcurrentExecutionResult<T>.AlreadyRunning();
      }
   }

   /// <summary>
   /// Ensures that only one call to this method will result in the action being executed. 
   /// </summary>
   /// <param name="action"></param>
   /// <returns></returns>
   public async Task<NoConcurrentExecutionStatus> Run(Func<Task> action)
   {
      if (Interlocked.Exchange(ref _isRunning, 1) == 0)
      {
         try
         {
            await action();
            return NoConcurrentExecutionStatus.Ran;
         }
         catch
         {
            return NoConcurrentExecutionStatus.Error;
         }
         finally
         {
            Interlocked.Exchange(ref _isRunning, 0);
         }
      }
      else
      {
         return NoConcurrentExecutionStatus.AlreadyRunning;
      }
   }

   /// <summary>
   /// Ensures that only one call to this method will result in the action being executed. 
   /// </summary>
   /// <param name="action"></param>
   /// <returns></returns>
   public async Task<NoConcurrentExecutionResult<T>> Run<T>(Func<Task<T>> action) where T : class?
   {
      if (Interlocked.Exchange(ref _isRunning, 1) == 0)
      {
         try
         {
            var result = await action();
            return NoConcurrentExecutionResult<T>.Ran(result);
         }
         catch
         {
            return NoConcurrentExecutionResult<T>.Error();
         }
         finally
         {
            Interlocked.Exchange(ref _isRunning, 0);
         }
      }
      else
      {
         return NoConcurrentExecutionResult<T>.AlreadyRunning();
      }
   }
}

public class NoConcurrentExecutionResult<T> where T : class
{
   public NoConcurrentExecutionStatus NoConcurrentExecutionStatus { get; set; }

   public Option<T> Value { get; set; }

   private NoConcurrentExecutionResult(NoConcurrentExecutionStatus invocationResult, T result)
   {
      NoConcurrentExecutionStatus = invocationResult;

      Value = result is null ? Option.None<T>() : result.Some();
   }

   private NoConcurrentExecutionResult(NoConcurrentExecutionStatus invocationResult)
   {
      NoConcurrentExecutionStatus = invocationResult;
      Value                        = Option.None<T>();
   }

   public static NoConcurrentExecutionResult<T> Ran(T result)
   {
      return new NoConcurrentExecutionResult<T>(NoConcurrentExecutionStatus.Ran, result);
   }

   public static NoConcurrentExecutionResult<T> AlreadyRunning()
   {
      return new NoConcurrentExecutionResult<T>(NoConcurrentExecutionStatus.AlreadyRunning);
   }

   public static NoConcurrentExecutionResult<T> Error()
   {
      return new NoConcurrentExecutionResult<T>(NoConcurrentExecutionStatus.Error);
   }
}