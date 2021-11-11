using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
        var bufferSize  = 4096;

        using (var sourceStream = 
               new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions))

        using (var destinationStream = 
               new FileStream(destinationFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, bufferSize, fileOptions))

            await sourceStream.CopyToAsync(destinationStream, bufferSize, cancellationToken)
                .ConfigureAwait(continueOnCapturedContext: false);
    }
}