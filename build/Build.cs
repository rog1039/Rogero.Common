using System;
using System.Collections.Generic;
using System.Linq;
using _Build;
using Microsoft.Build.Tasks;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Perform a full nuget delete and reinstall")] readonly bool ReinstallAllPackages = false;

    [Solution]      readonly Solution      Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion]    readonly GitVersion    GitVersion;

    AbsolutePath SourceDirectory          => RootDirectory / "src";
    AbsolutePath ArtifactsDirectory       => RootDirectory / "artifacts";
    AbsolutePath NugetPackagesDirectory   => Solution.Directory / "packages";
    AbsolutePath NugetPackOutputDirectory => Solution.Directory / "artifacts";

    Project RogeroCommonProject => Solution.GetProject("Rogero.Common");
    string  MyGetPrivateFeedUrl = "https://www.myget.org/F/progerop/auth/08c5805d-ebbb-4edf-82f9-279dd8a6d16d/api/v3/index.json";
    string  MyGetPublicFeedUrl  = "https://www.myget.org/F/progero/api/v3/index.json";
    string  ConfigurationString => Configuration == Configuration.Debug ? "Debug" : "Release";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
            if (ReinstallAllPackages) EnsureCleanDirectory(NugetPackagesDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            MSBuild(s => s
                        .SetTargetPath(Solution)
                        .AddRestoreSources(MyGetPrivateFeedUrl, MyGetPublicFeedUrl)
                        .SetTargets("Restore"));

            if (ReinstallAllPackages)
                NuGetTasks
                    .NuGetRestore(s => s
                                      .SetTargetPath(Solution)
                                      .SetSolutionDirectory(SourceDirectory)
                                      .SetOutputDirectory(NugetPackagesDirectory));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .DependsOn(Clean)
        .Executes(() =>
        {
            ;

            MSBuild(s => s
                        .SetTargetPath(Solution)
                        //.SetTargets("Rebuild")
                        .SetConfiguration(Configuration)
                        .SetMaxCpuCount(Environment.ProcessorCount)
                        .SetNodeReuse(IsLocalBuild));
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .DependsOn(Clean)
        .Executes(() =>
        {
            var version = new MyVersion(GitVersion);
            //
            //

            //DotNetTasks
            //    .DotNetPack(s => s
            //                    .SetProject(RogeroCommonProject.Path)
            //                    .SetConfiguration(Configuration.ToString())
            //                    .SetIncludeSource(true)
            //                    .SetIncludeSymbols(true)


            //    );


            NuGetTasks
                .NuGetPack(s => s
                               .SetVersion(version.GetAssemblyVersion())
                               .SetOutputDirectory(ArtifactsDirectory)
                               .EnableSymbols()
                               .SetProperties(new Dictionary<string, object>
                               {
                                   {"Configuration", Configuration.ToString()},
                                   {"Platform", "AnyCPU"}
                               })
                               .SetWorkingDirectory(Solution.Directory)
                               .SetSymbolPackageFormat(NuGetSymbolPackageFormat.symbols_nupkg)
                               .SetTargetPath(RogeroCommonProject.Path)
                );
        });
}