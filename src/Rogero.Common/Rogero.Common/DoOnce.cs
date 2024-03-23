namespace Rogero.Common;

/// <summary>
///     A thread-safe class to ensure an action is only performed one time.
/// </summary>
public class DoOnce
{
   private       long _statusBit = 0; 
   private const long NotRunning = 0;
   private const long Running    = 1;
   private const long Completed  = 2;
   private const long Errored    = 3;

   /// <summary>
   ///     Runs the provided action only if this method has never been called before.
   /// </summary>
   /// <param name="action">The action to perform.</param>
   /// <param name="resetOnException">
   ///     If true, then if an exception occurs while running the action then the status of this
   ///     gate will be as if the action was never performed
   /// </param>
   /// <returns>True if the action was done, false if the action was not done.</returns>
   public bool Ensure(Action action, bool resetOnException)
   {
      var shouldDo = ShouldDo();
      if (shouldDo)
      {
         PerformAction(action, resetOnException);
      }

      return shouldDo;
   }

   /// <summary>
   ///     Returns true if the method has never been called before, returns false if the method has been called.
   /// </summary>
   /// <returns></returns>
   private bool ShouldDo()
   {
      return Interlocked.Exchange(ref _statusBit, Running) == NotRunning;
   }

   private void PerformAction(Action action, bool resetOnException)
   {
      try
      {
         action();
         Interlocked.Exchange(ref _statusBit, Completed);
      }
      catch (Exception exception)
      {
         if (resetOnException)
         {
            Reset();
         }
         else
         {
            Interlocked.Exchange(ref _statusBit, Errored);
         }

         throw;
      }
   }

   /// <summary>
   ///     Resets this object as if the action has never been performed.
   /// </summary>
   public void Reset()
   {
      Interlocked.Exchange(ref _statusBit, NotRunning);
   }

   public DoOnceStatus Status
   {
      get
      {
         var intStatus = Interlocked.Read(ref _statusBit);
         return (DoOnceStatus) intStatus;
      }
   }
}

public enum DoOnceStatus
{
   NotRun   = 0,
   Running  = 1,
   Complete = 2,
   Errored  = 3
}