#I "packages/FAKE/tools"
#r "packages/FAKE/tools/FakeLib.dll"

open Fake

// Dirs

let tempDir = "./temp"
let srcDir = tempDir + "/src"

// Clean

Target "Clean" (fun _ ->
    CleanDirs [ tempDir ])

// Build

Target "Build" (fun _ ->
    !! "src/**/*.fsproj"
    |> MSBuildRelease srcDir "Build" 
    |> Log "Build Source: ")

// Publish

Target "Publish" (fun _ ->
    NuGet (fun p ->
        { p with
              Authors = [ "Andrew Cherry" ]
              Project = "SemVer.Net"
              OutputPath = tempDir
              WorkingDir = srcDir
              Version = "2.0.0"
              AccessKey = getBuildParamOrDefault "nuget_key" ""
              Publish = hasBuildParam "nuget_key"
              Dependencies =
                [ "FParsec", GetPackageVersion "packages" "FParsec"
                  "FSharp.Core", GetPackageVersion "packages" "FSharp.Core" ]
              Files = 
                [ "SemVer.dll", Some "lib/net40", None ] })
              "./nuget/SemVer.nuspec")

// Dependencies

"Clean"
    ==> "Build"
    ==> "Publish"

RunTargetOrDefault "Publish"
