//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    try {
        DeleteFiles("./**/bin/**/**/**");
        DeleteFiles("./**/bin/**/**");
        CleanDirectories("./**/bin/**/**");
        CleanDirectories("./**/bin/**");
    } catch (Exception ex) {
        Information($"Failed to clean: {ex}");
    }
});

Task("Restore-NuGet-Packages")
    .Does(() =>
{
    DotNetCoreRestore("./NewsCrawler.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    DotNetCoreBuild("NewsCrawler.sln", new DotNetCoreBuildSettings {
        Verbosity = DotNetCoreVerbosity.Minimal,
        Configuration = configuration
    });
});

Task("Test")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreTest("NewsCrawler.sln");
});

Task("Run")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreExecute($"./NewsCrawler/bin/{buildDir}/netcoreapp2.1/NewsCrawler.dll");
});

Task("ProdRun")
    .IsDependentOn("Build")
    .Does(() =>
{
    CopyFile("appsettings.json", $"./NewsCrawler/bin/{buildDir}/netcoreapp2.1/appsettings.json");
    DotNetCoreExecute($"./NewsCrawler/bin/{buildDir}/netcoreapp2.1/NewsCrawler.dll");
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
