namespace Rogero.Common.Infrastructure;

public static class PerformanceTimer
{
    public static PerformanceTimerManager Start()
    {
        return new PerformanceTimerManager();
    }

    public class PerformanceTimerManager : IDisposable
    {
        public DateTime StartTime          { get; set; }
        public DateTime LastCheckpointTime { get; set; }
            
        private readonly Action<string> _loggerAction = Console.WriteLine;

        public PerformanceTimerManager()
        {
            StartTime = LastCheckpointTime = DateTime.UtcNow;
        }

        public PerformanceTimerManager(Action<string> customLoggerAction)
        {
            StartTime     = LastCheckpointTime = DateTime.UtcNow;
            _loggerAction = customLoggerAction;
        }

        public void Checkpoint(string checkpointName)
        {
            var now                        = DateTime.UtcNow;
            var totalElapsed               = now - StartTime;
            var elapsedSinceLastCheckpoint = now - LastCheckpointTime;
            LastCheckpointTime = now;
            _loggerAction($"{checkpointName}: {totalElapsed.TotalMilliseconds} [{elapsedSinceLastCheckpoint.TotalMilliseconds}]");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Checkpoint("Finished");
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}