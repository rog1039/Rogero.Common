using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace Rogero.Common.Tests;

public class FileSystemHelperTests : UnitTestBaseWithConsoleRedirection
{
    [Fact()]
    [Trait("Category", "Instant")]
    public async Task TestCopyFileSpeed()
    {
        var sw  = new Stopwatch();
        sw.Start();
        var src = @"\\woebermustard.com\files\Redirection\paulrogero\My Documents\VeeamRecoveryMedia_D9020W88GBN1-RicksPC.iso";
        var dst = @"C:\users\paulrogero\target.iso";
        await FileSystemHelpers.CopyFileAsync(src, dst, CancellationToken.None);
        sw.Stop();
        Console.WriteLine(sw.Elapsed);

        var fileInfo          = new FileInfo(src);
        var megabytes         = fileInfo.Length / 1024 / 1024;
        var megabits          = megabytes * 8;
        var megabitsPerSecond = megabits / sw.Elapsed.TotalSeconds;
        var megabytesPerSecond = megabytes / sw.Elapsed.TotalSeconds;

        Console.WriteLine($"Megabytes: {megabytes}");
        Console.WriteLine($"Megabits: {megabits}");
        Console.WriteLine($"{megabitsPerSecond} mb/sec");
        Console.WriteLine($"{megabytesPerSecond} MB/sec");
        
        File.Delete(dst);
    }

    public FileSystemHelperTests(ITestOutputHelper outputHelperHelper) : base(outputHelperHelper) { }
}