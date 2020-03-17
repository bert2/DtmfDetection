#tool GitVersion.CommandLine
#tool Codecov
#addin Cake.Codecov
#addin Cake.Git
#load prompt.cake
#load format-rel-notes.cake

var target = Argument("target", "Default");
var config = Argument("configuration", "Release");
var nugetKey = Argument<string>("nugetKey", null) ?? EnvironmentVariable("nuget_key");

var rootDir = Directory("..");
var testDir = rootDir + Directory("test");

var lastCommitMsg = EnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE") ?? GitLogTip(rootDir).MessageShort;
var lastCommitSha = EnvironmentVariable("APPVEYOR_REPO_COMMIT") ?? GitLogTip(rootDir).Sha;
var currBranch = GitBranchCurrent(rootDir).FriendlyName;
GitVersion semVer = null;

Task("SemVer")
    .Does(() => {
        semVer = GitVersion();
        Information($"{semVer.FullSemVer} ({lastCommitMsg})");
    });

Task("Clean")
    .Does(() =>
        DotNetCoreClean(rootDir, new DotNetCoreCleanSettings {
            Configuration = config,
            Verbosity = DotNetCoreVerbosity.Minimal
        }));

Task("Build")
    .IsDependentOn("SemVer")
    .Does(() =>
        DotNetCoreBuild(rootDir, new DotNetCoreBuildSettings {
            Configuration = config,
            MSBuildSettings = new DotNetCoreMSBuildSettings()
                .SetVersion(semVer.AssemblySemVer)
        }));

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
        DotNetCoreTest(rootDir, new DotNetCoreTestSettings {
            Configuration = config,
            NoBuild = true,
            ArgumentCustomization = args => {
                var msbuildSettings = new DotNetCoreMSBuildSettings()
                    .WithProperty("CollectCoverage", new[] { "true" })
                    .WithProperty("CoverletOutputFormat", new[] { "opencover" });
                args.AppendMSBuildSettings(msbuildSettings, environment: null);
                return args;
            }
        }));

Task("UploadCoverage")
    .Does(() => {
        Codecov(testDir + File("unit/coverage.opencover.xml"));
        Codecov(testDir + File("integration/coverage.opencover.xml"));
    });

Task("Pack-DtmfDetection")
    .IsDependentOn("SemVer")
    .Does(() => {
        var relNotes = FormatReleaseNotes(lastCommitMsg);
        Information($"Packing {semVer.NuGetVersion} ({relNotes})");

        var pkgName = "DtmfDetection";
        var pkgDesc = $"Implementation of the Goertzel algorithm for the detection of DTMF tones (aka touch tones) in audio data. Install the package \"DtmfDetection.NAudio\" for integration with NAudio.\r\n\r\nDocumentation: https://github.com/bert2/DtmfDetection\r\n\r\nRelease notes: {relNotes}";
        var pkgTags = "Goertzel; DTMF; touch tone; detection; dsp; signal processing; audio analysis";
        var libDir = rootDir + Directory($"src/{pkgName}");
        var pkgDir = libDir + Directory($"bin/{config}");

        var msbuildSettings = new DotNetCoreMSBuildSettings();
        msbuildSettings.Properties["PackageId"]                = new[] { pkgName };
        msbuildSettings.Properties["PackageVersion"]           = new[] { semVer.NuGetVersion };
        msbuildSettings.Properties["Title"]                    = new[] { pkgName };
        msbuildSettings.Properties["Description"]              = new[] { pkgDesc };
        msbuildSettings.Properties["PackageTags"]              = new[] { pkgTags };
        msbuildSettings.Properties["PackageReleaseNotes"]      = new[] { relNotes };
        msbuildSettings.Properties["Authors"]                  = new[] { "Robert Hofmann" };
        msbuildSettings.Properties["RepositoryUrl"]            = new[] { "https://github.com/bert2/DtmfDetection.git" };
        msbuildSettings.Properties["RepositoryCommit"]         = new[] { lastCommitSha };
        msbuildSettings.Properties["PackageLicenseExpression"] = new[] { "MIT" };
        msbuildSettings.Properties["IncludeSource"]            = new[] { "true" };
        msbuildSettings.Properties["IncludeSymbols"]           = new[] { "true" };
        msbuildSettings.Properties["SymbolPackageFormat"]      = new[] { "snupkg" };

        DotNetCorePack(libDir, new DotNetCorePackSettings {
            Configuration = config,
            OutputDirectory = pkgDir,
            NoBuild = true,
            NoDependencies = false,
            MSBuildSettings = msbuildSettings
        });
    });

Task("Pack-DtmfDetection.NAudio")
    .IsDependentOn("SemVer")
    .Does(() => {
        var relNotes = FormatReleaseNotes(lastCommitMsg);
        Information($"Packing {semVer.NuGetVersion} ({relNotes})");

        var pkgName = "DtmfDetection.NAudio";
        var pkgDesc = $"Extends NAudio with means to detect DTMF tones (aka touch tones) in live audio data and audio files.\r\n\r\nDocumentation: https://github.com/bert2/DtmfDetection\r\n\r\nRelease notes: {relNotes}";
        var pkgTags = "Goertzel; DTMF; touch tone; detection; dsp; signal processing; audio analysis; NAudio";
        var libDir = rootDir + Directory($"src/{pkgName}");
        var pkgDir = libDir + Directory($"bin/{config}");

        var msbuildSettings = new DotNetCoreMSBuildSettings();
        msbuildSettings.Properties["PackageId"]                = new[] { pkgName };
        msbuildSettings.Properties["PackageVersion"]           = new[] { semVer.NuGetVersion };
        msbuildSettings.Properties["Title"]                    = new[] { pkgName };
        msbuildSettings.Properties["Description"]              = new[] { pkgDesc };
        msbuildSettings.Properties["PackageTags"]              = new[] { pkgTags };
        msbuildSettings.Properties["PackageReleaseNotes"]      = new[] { relNotes };
        msbuildSettings.Properties["Authors"]                  = new[] { "Robert Hofmann" };
        msbuildSettings.Properties["RepositoryUrl"]            = new[] { "https://github.com/bert2/DtmfDetection.git" };
        msbuildSettings.Properties["RepositoryCommit"]         = new[] { lastCommitSha };
        msbuildSettings.Properties["PackageLicenseExpression"] = new[] { "MIT" };
        msbuildSettings.Properties["IncludeSource"]            = new[] { "true" };
        msbuildSettings.Properties["IncludeSymbols"]           = new[] { "true" };
        msbuildSettings.Properties["SymbolPackageFormat"]      = new[] { "snupkg" };

        DotNetCorePack(libDir, new DotNetCorePackSettings {
            Configuration = config,
            OutputDirectory = pkgDir,
            NoBuild = true,
            NoDependencies = false,
            MSBuildSettings = msbuildSettings
        });
    });

Task("Release-DtmfDetection")
    .IsDependentOn("Pack-DtmfDetection")
    .Does(() => {
        if (currBranch != "master") {
            Information($"Will not release package built from branch '{currBranch}'.");
            return;
        }

        if (lastCommitMsg.Contains("without release")) {
            Information($"Skipping release to nuget.org");
            return;
        }

        Information($"Releasing {semVer.NuGetVersion} to nuget.org");

        if (string.IsNullOrEmpty(nugetKey))
            nugetKey = Prompt("Enter nuget API key: ");

        var pkgName = "DtmfDetection";
        var pkgDir = rootDir + Directory($"src/{pkgName}/bin/{config}");

        DotNetCoreNuGetPush(
            pkgDir + File($"{pkgName}.{semVer.NuGetVersion}.nupkg"),
            new DotNetCoreNuGetPushSettings {
                Source = "nuget.org",
                ApiKey = nugetKey
            });
    });

Task("Release-DtmfDetection.NAudio")
    .IsDependentOn("Pack-DtmfDetection.NAudio")
    .Does(() => {
        if (currBranch != "master") {
            Information($"Will not release package built from branch '{currBranch}'.");
            return;
        }

        if (lastCommitMsg.Contains("without release")) {
            Information($"Skipping release to nuget.org");
            return;
        }

        Information($"Releasing {semVer.NuGetVersion} to nuget.org");

        if (string.IsNullOrEmpty(nugetKey))
            nugetKey = Prompt("Enter nuget API key: ");

        var pkgName = "DtmfDetection.NAudio";
        var pkgDir = rootDir + Directory($"src/{pkgName}/bin/{config}");

        DotNetCoreNuGetPush(
            pkgDir + File($"{pkgName}.{semVer.NuGetVersion}.nupkg"),
            new DotNetCoreNuGetPushSettings {
                Source = "nuget.org",
                ApiKey = nugetKey
            });
    });

Task("Default")
    .IsDependentOn("SemVer")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

Task("Pack")
    .IsDependentOn("Pack-DtmfDetection")
    .IsDependentOn("Pack-DtmfDetection.NAudio");

Task("Release")
    .IsDependentOn("UploadCoverage")
    .IsDependentOn("Release-DtmfDetection")
    .IsDependentOn("Release-DtmfDetection.NAudio");

RunTarget(target);
