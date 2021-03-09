using System;
using System.Threading;

namespace Rogero.Common
{
    /// <summary>
    ///     A thread-safe class to ensure an action is only performed one time.
    /// </summary>
    public class DoOnce
    {
        private long _isDone = 0; //0 is false, 1 is true.

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
            if (Interlocked.Exchange(ref _isDone, 1) == 0)
                return true;
            else
                return false;
        }

        private void PerformAction(Action action, bool resetOnException)
        {
            try
            {
                action();
                Interlocked.Exchange(ref _isDone, 2);
            }
            catch (Exception exception)
            {
                if (resetOnException)
                {
                    Reset();
                }
                else
                {
                    Interlocked.Exchange(ref _isDone, 3);
                }

                throw;
            }
        }

        /// <summary>
        ///     Resets this object as if the action has never been performed.
        /// </summary>
        private void Reset()
        {
            Interlocked.Exchange(ref _isDone, 0);
        }

        public DoOnceStatus Status
        {
            get
            {
                var intStatus = Interlocked.Read(ref _isDone);
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
}