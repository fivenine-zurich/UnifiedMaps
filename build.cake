#tool nuget:?package=GitVersion.CommandLine

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(Directory("./bin"));
    CleanDirectory(Directory("./obj"));
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./UnifiedMaps.sln");
});

Task("UpdateAssemblyInfo")
    .Does(() =>
{
    var versionInfo = GitVersion(new GitVersionSettings {
        UpdateAssemblyInfo = true
    });

    Information("Version: {0}", versionInfo.FullSemVer);
    Information("NuGet Version: {0}", versionInfo.NuGetVersionV2);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./UnifiedMaps.sln", settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild("./UnifiedMaps.sln", settings =>
        settings.SetConfiguration(configuration));
    }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

Task("Build-Dist")
    .IsDependentOn("UpdateAssemblyInfo")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
