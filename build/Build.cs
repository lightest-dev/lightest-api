using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.TravisCI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
internal class Build : NukeBuild
{
    /// Support plugins are available for:
    /// - JetBrains ReSharper https://nuke.build/resharper
    /// - JetBrains Rider https://nuke.build/rider
    /// - Microsoft VisualStudio https://nuke.build/visualstudio
    /// - Microsoft VSCode https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] private readonly Solution Solution;
    [GitRepository] private readonly GitRepository GitRepository;
    [CI] private readonly TravisCI TravisCi;

    private string Version => $"0.10.1.{TravisCi?.BuildNumber ?? 0}";

    private AbsolutePath SourceDirectory => RootDirectory / "src";
    private AbsolutePath TestsDirectory => RootDirectory / "test";
    private AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    private AbsolutePath CoveragePath => ArtifactsDirectory / "cover.xml";

    private Target Clean => _ => _
         .Before(Restore)
         .Executes(() =>
         {
             SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
             TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
             EnsureCleanDirectory(ArtifactsDirectory);
         });

    private Target Restore => _ => _
         .Executes(() =>
         {
             DotNetRestore(s => s
                 .SetProjectFile(Solution));
         });

    private Target Compile => _ => _
         .DependsOn(Restore)
         .Executes(() =>
         {
             DotNetBuild(s => s
                 .SetProjectFile(Solution)
                 .SetConfiguration(Configuration)
                 .SetAssemblyVersion(Version)
                 .SetFileVersion(Version)
                 .EnableNoRestore());
         });

    private Target Test => _ => _
         .DependsOn(Compile)
         .Executes(() =>
         {
             var testSettings = new DotNetTestSettings()
                 .SetProjectFile(Solution)
                 .SetConfiguration(Configuration)
                 .EnableNoBuild()
                 .EnableNoRestore()
                 .SetArgumentConfigurator(arguments => arguments
                     .Add("/p:CollectCoverage={0}", true)
                     .Add("/p:CoverletOutput={0}", CoveragePath)
                     .Add("/p:CoverletOutputFormat={0}", "opencover")
                     .Add("/p:Exclude=\"{0}\"",
                         ("[Lightest.Data*]*,[*]*Models*,[*]*Migrations*," +
                         "[Lightest.AccessService]Lightest.AccessService.MockAccessServices.*," +
                         "[xunit.*]*,[*]*.Seed,[*]*.Startup,[*]*.ServiceRegistrator,[*]*.Program")
                         .Replace(",", "%2c")));

             DotNetTest(testSettings);
         });
}
