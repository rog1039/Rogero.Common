using System.Reflection;
using Optional;

namespace Rogero.Common;

public static class FileSystemHelpers
{
    public static Option<FileInfo> FindFileUpwards(string fileName, int upwardIterationLimit)
    {
        var executingAssemblyPath = Assembly.GetExecutingAssembly().Location;
        var startingDirectory     = Path.GetDirectoryName(executingAssemblyPath);
        return FindFileUpwards(startingDirectory, fileName, upwardIterationLimit);
    }
        
    public static Option<FileInfo> FindFileUpwards(string startingDirectory, string fileName, int upwardIterationLimit)
    {
        var directory = new DirectoryInfo(startingDirectory);
            
        for (int i = -1; i < upwardIterationLimit; i++)
        {
            var files = directory.EnumerateFiles(fileName);
                
            var firstFile = files.FirstOrDefault();
            if (firstFile != null) return firstFile.Some();

            if (directory.Parent == null)
                return Option.None<FileInfo>();

            directory = directory.Parent;
        }

        return Option.None<FileInfo>();
    }
        
    public static async Task CopyFileAsync(string sourceFile, string destinationFile, CancellationToken cancellationToken)
    {
        var fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
        
        /*
         * Used unit testing to check different buffer sizes.
         * 4096 performed about 150mbit/sec
         * 4096*16 performed about 900 mbit/sec
         * 4096*32 performed about 1200 mbit/sec
         * 4096*48 performed about 1020 mbit/sec
         * 4096*64 performed about 1000 mbit/sec
         */
        var bufferSize  = 4096*32;

        await using var sourceStream = 
            new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions);
        await using var destinationStream = 
            new FileStream(destinationFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, bufferSize, fileOptions);
        
        await sourceStream
            .CopyToAsync(destinationStream, bufferSize, cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);
    }
}