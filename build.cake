//////////////////////////////////////////////////////////////////////
// Cake build script for ResizeX
//////////////////////////////////////////////////////////////////////

#addin nuget:?package=Cake.7zip&version=3.0.0
#addin nuget:?package=Cake.FileHelpers&version=6.1.3

var artifacts = Argument("artifacts", "artifacts");
var libartifacts = Argument("libartifacts", "lib");
var nugetartifacts = Argument("nugetartifacts", "nuget");
var configuration = Argument("configuration", "Release");
var framework = Argument("framework", "net6.0");
var runtime = Argument("runtime", "win-x64");
var target = Argument("target", "ZipRelease");
IEnumerable<string> IGitVersion;
string SGitVersion;
string sAssemblyVersion;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("AssemblyVersion")
    .Does(() =>
    {
        if (FileExists("./ResizeX/Program.cs"))
        {
            Console.WriteLine(Environment.NewLine + "Reading assembly version from ResizeX.csproj");
            sAssemblyVersion = FindRegexMatchInFile("./ResizeX/ResizeX.csproj", 
                                "<AssemblyVersion>(.*?)</AssemblyVersion>",
                                System.Text.RegularExpressions.RegexOptions.None
                                );
            sAssemblyVersion = sAssemblyVersion.Replace("<AssemblyVersion>","").Replace("</AssemblyVersion>","").Replace("0.0.","");
            Console.WriteLine(Environment.NewLine + $"AssemblyVersion version: {sAssemblyVersion}");
        }
    });

Task("GitVersion")
    .Does(() =>
    {
    var exitCodeWithArgument = StartProcess
        (
            "git",
            new ProcessSettings{
                Arguments = "rev-parse --short HEAD",
                RedirectStandardOutput = true
            },
            out IGitVersion
        );
    SGitVersion = string.Join("", IGitVersion).Trim();
    if (SGitVersion == "") SGitVersion = "0";
    SGitVersion = "git-" + SGitVersion;
    Console.WriteLine(Environment.NewLine + $"Git version: {SGitVersion}");
    });

Task("CleanAll")
        .IsDependentOn("Clean")
        .IsDependentOn("CleanArtifacts")
    ;

Task("Clean")
    .Does(() =>
    {
        Console.WriteLine(Environment.NewLine + $"Cleaning folder: ./ResizeX/bin/{configuration}");
        CleanDirectory($"./ResizeX/bin/{configuration}");
        Console.WriteLine(Environment.NewLine + $"Cleaning folder: ./Feuster.Imaging.Resizing.Lib/bin/{configuration}");
        CleanDirectory($"./Feuster.Imaging.Resizing.Lib/bin/{configuration}");
    });

Task("CleanArtifacts")
    .Does(() =>
    {
        Console.WriteLine(Environment.NewLine + $"Cleaning folder: ./{libartifacts}");
        CleanDirectory($"./{libartifacts}");
        Console.WriteLine(Environment.NewLine + $"Cleaning folder: ./{nugetartifacts}");
        CleanDirectory($"./{nugetartifacts}");
        Console.WriteLine(Environment.NewLine + $"Cleaning folder: ./{artifacts}");
        CleanDirectory($"./{artifacts}");
    });

Task("RegexFiles")
    .IsDependentOn("GitVersion")
    .Does(() =>
    {
        if (FileExists("./Feuster.Imaging.Resizing.Lib/Feuster.Imaging.Resizing.Lib.cs"))
        {
            Console.WriteLine(Environment.NewLine + "Patching nuget artifact path in Feuster.Imaging.Resizing.Lib.csproj");
            ReplaceRegexInFiles("./Feuster.Imaging.Resizing.Lib/Feuster.Imaging.Resizing.Lib.csproj", 
                                "<PackageOutputPath>(.*?)</PackageOutputPath>", 
                                $"<PackageOutputPath>./../{nugetartifacts}</PackageOutputPath>");
        }

        if (FileExists("./Feuster.Imaging.Resizing.Lib/Feuster.Imaging.Resizing.Lib.cs"))
        {
            Console.WriteLine(Environment.NewLine + "Patching git revision in Feuster.Imaging.Resizing.Lib.cs");
            ReplaceRegexInFiles("./Feuster.Imaging.Resizing.Lib/Feuster.Imaging.Resizing.Lib.cs", 
                                "const string GitVersion = \"(.*?)\"", 
                                $"const string GitVersion = \"{SGitVersion}\"");
        }

        if (FileExists("./ResizeX/Program.cs"))
        {
            Console.WriteLine(Environment.NewLine + "Patching git revision in Program.cs");
            ReplaceRegexInFiles("./ResizeX/Program.cs", 
                                "const string GitVersion = \"(.*?)\"", 
                                $"const string GitVersion = \"{SGitVersion}\"");
        }
    });

Task("Build")
    .IsDependentOn("CleanAll")
    .IsDependentOn("RegexFiles")
    .Does(() =>
    {
        DotNetBuild("./Feuster.Imaging.Resizing.Lib/Feuster.Imaging.Resizing.Lib.csproj", new DotNetBuildSettings
        {
            Configuration = configuration,
        });

        DotNetBuild("./ResizeX/ResizeX.csproj", new DotNetBuildSettings
        {
            Configuration = configuration,
        });
    });

Task("Publish")
    .IsDependentOn("CleanAll")
    .IsDependentOn("RegexFiles")
    .Does(() =>
    {
        Console.WriteLine();
        DotNetPublish("./Feuster.Imaging.Resizing.Lib/Feuster.Imaging.Resizing.Lib.csproj", new DotNetPublishSettings
        {
            Configuration = configuration,
            Framework = framework,
            OutputDirectory = $"./{libartifacts}/",
            Runtime = runtime,
            SelfContained = false
        });

        Console.WriteLine();
        DotNetPublish("./ResizeX/ResizeX.csproj", new DotNetPublishSettings
        {
            Configuration = configuration,
            EnableCompressionInSingleFile = false,
            Framework = framework,
            OutputDirectory = $"./{artifacts}/",
            PublishSingleFile = true,
            PublishReadyToRun = false,
            PublishTrimmed = false,
            PublishReadyToRunShowWarnings = true,
            Runtime = runtime,
            SelfContained = true
        });
    });


Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        DotNetTest("./ResizeX.sln", new DotNetTestSettings
        {
            Configuration = configuration,
            NoBuild = true,
        });
    });

Task("ZipRelease")
    .IsDependentOn("AssemblyVersion")
	.IsDependentOn("Build")
    .IsDependentOn("Publish")
    .Does(() =>
    {
        var WorkDir = Context.Environment.WorkingDirectory;

        if (FileExists($"{WorkDir}/Release/Feuster.Imaging.Resizing.Lib_{runtime}_{SGitVersion}.zip"))
        {
            Console.Write(Environment.NewLine + $"Deleting existing Feuster.Imaging.Resizing.Lib_{runtime}_{SGitVersion}.zip");
            DeleteFile($"{WorkDir}/Release/Feuster.Imaging.Resizing.Lib_{runtime}_{SGitVersion}.zip");
        }
        if (FileExists($"{WorkDir}/Release/ResizeX_V{sAssemblyVersion}_{runtime}_{SGitVersion}.zip"))
        {
            Console.Write(Environment.NewLine + $"Deleting existing ResizeX_V{sAssemblyVersion}_{runtime}_{SGitVersion}.zip");
            DeleteFile($"{WorkDir}/Release/ResizeX_V{sAssemblyVersion}_{runtime}_{SGitVersion}.zip");
        }

        Console.Write(Environment.NewLine + Environment.NewLine + "Start Zipping ResizeX...");
        FilePathCollection files;
		files = new FilePathCollection(new[]
					{
						new FilePath($"{WorkDir}/gpl-2.0.txt"),
						new FilePath($"{WorkDir}/README.md")
					});
		DirectoryPathCollection directories;
		directories = new DirectoryPathCollection(new[]
					{
						new DirectoryPath($"{WorkDir}/{libartifacts}/"),
						new DirectoryPath($"{WorkDir}/{nugetartifacts}/")
					});
		SwitchCompressionMethod method = new SwitchCompressionMethod();
		method.Level = 9;
		method.Method = "Bzip2";
        SevenZip(new SevenZipSettings
        {
            Command = new AddCommand
            {
                Files = files,
				Directories = directories,
				CompressionMethod = method,
                Archive = new FilePath($"{WorkDir}/Release/Feuster.Imaging.Resizing.Lib_{runtime}_{SGitVersion}.zip"),
            }
        });
        Context.Environment.WorkingDirectory =  WorkDir;
        Console.WriteLine("finished!" + Environment.NewLine);
        if (FileExists($"{WorkDir}/Release/Feuster.Imaging.Resizing.Lib_{runtime}_{SGitVersion}.zip"))
            Console.WriteLine($"Feuster.Imaging.Resizing.Lib_{runtime}_{SGitVersion}.zip successfully created!");
        else
            Console.WriteLine($"Feuster.Imaging.Resizing.Lib_{runtime}_{SGitVersion}.zip creation failed!");

        Context.Environment.WorkingDirectory +=  $"/{artifacts}/";
        Console.Write(Environment.NewLine + "Start Zipping ResizeX...");
		files = new FilePathCollection(new[]
					{
						new FilePath($"./ResizeX.exe"),
						new FilePath($"{WorkDir}/gpl-2.0.txt"),
						new FilePath($"{WorkDir}/README.md")
					});
		directories = new DirectoryPathCollection(new[]
					{
						new DirectoryPath($"{WorkDir}/Examples/")
					});
		method.Level = 9;
		method.Method = "Bzip2";
        SevenZip(new SevenZipSettings
        {
            Command = new AddCommand
            {
                Files = files,
				Directories = directories,
				CompressionMethod = method,
                Archive = new FilePath($"{WorkDir}/Release/ResizeX_V{sAssemblyVersion}_{runtime}_{SGitVersion}.zip"),
            }
        });
        Context.Environment.WorkingDirectory =  WorkDir;
        Console.WriteLine("finished!" + Environment.NewLine);
        if (FileExists($"{WorkDir}/Release/ResizeX_V{sAssemblyVersion}_{runtime}_{SGitVersion}.zip"))
            Console.WriteLine($"ResizeX_V{sAssemblyVersion}_{runtime}_{SGitVersion}.zip successfully created!");
        else
            Console.WriteLine($"ResizeX_V{sAssemblyVersion}_{runtime}_{SGitVersion}.zip creation failed!");
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);