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

    var publishSettings = new DotNetCorePublishSettings
    {
        Configuration = configuration
    };
    DotNetCorePublish("./NewsCrawler.WebUI/NewsCrawler.WebUI.csproj", publishSettings);
    DotNetCorePublish("./NewsCrawler/NewsCrawler.csproj", publishSettings);
});

Task("Test")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreTest("NewsCrawler.sln");
});

Task("TitleUpdate")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreExecute($"./NewsCrawler/bin/{buildDir}/netcoreapp2.2/NewsCrawler.dll", "Title-Update");
});

Task("ProdTitleUpdate")
    .IsDependentOn("Build")
    .Does(() =>
{
    CopyFile("Docker/Crawler/appsettings.json", $"./NewsCrawler/bin/{buildDir}/netcoreapp2.2/appsettings.json");
    DotNetCoreExecute($"./NewsCrawler/bin/{buildDir}/netcoreapp2.2/NewsCrawler.dll", "Title-Update");
});

Task("CleanArticles")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreExecute($"./NewsCrawler/bin/{buildDir}/netcoreapp2.2/NewsCrawler.dll", "Clean-Article");
});

Task("ProdCleanArticles")
    .IsDependentOn("Build")
    .Does(() =>
{
    CopyFile("Docker/Crawler/appsettings.json", $"./NewsCrawler/bin/{buildDir}/netcoreapp2.2/appsettings.json");
    DotNetCoreExecute($"./NewsCrawler/bin/{buildDir}/netcoreapp2.2/NewsCrawler.dll", "Clean-Article");
});

Task("UpdateWordCount")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreExecute($"./NewsCrawler/bin/{buildDir}/netcoreapp2.2/NewsCrawler.dll", "Update-Word-Count");
});

Task("ProdUpdateWordCount")
    .IsDependentOn("Build")
    .Does(() =>
{
    CopyFile("Docker/Crawler/appsettings.json", $"./NewsCrawler/bin/{buildDir}/netcoreapp2.2/appsettings.json");
    DotNetCoreExecute($"./NewsCrawler/bin/{buildDir}/netcoreapp2.2/NewsCrawler.dll", "Update-Word-Count");
});

Task("Run")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreExecute($"./NewsCrawler/bin/{buildDir}/netcoreapp2.2/NewsCrawler.dll");
});

Task("ProdBuild")
    .IsDependentOn("Build")
    .Does(() =>
{
    CopyFile("Docker/Crawler/appsettings.json", $"./NewsCrawler/bin/{buildDir}/netcoreapp2.2/publish/appsettings.json");
    CopyFile("./NewsCrawler.WebUI/appsettings.docker.json",
        $"./NewsCrawler.WebUI/bin/{buildDir}/netcoreapp2.2/publish/appsettings.json");
});
Task("ProdRun")
    .IsDependentOn("Build")
    .Does(() =>
{
    CopyFile("Docker/Crawler/appsettings.json", $"./NewsCrawler/bin/{buildDir}/netcoreapp2.2/publish/appsettings.json");
    DotNetCoreExecute($"./NewsCrawler/bin/{buildDir}/netcoreapp2.2/publish/NewsCrawler.dll");
});

Task("WebUI")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreExecute($"./NewsCrawler.WebUI/bin/{buildDir}/netcoreapp2.2/publish/NewsCrawler.WebUI.dll");
});

Task("ProdWebUI")
    .IsDependentOn("ProdBuild")
    .Does(() =>
{
    var publishDirectory = $"./NewsCrawler.WebUI/bin/{buildDir}/netcoreapp2.2/publish";
    var executeSettings = new DotNetCoreExecuteSettings
    {
        WorkingDirectory = publishDirectory
    };

    DotNetCoreExecute($"./NewsCrawler.WebUI/bin/{buildDir}/netcoreapp2.2/publish/NewsCrawler.WebUI.dll", string.Empty, executeSettings);
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
