#I "packages/FAKE/tools"
#r "packages/FAKE/tools/FakeLib.dll"

open Fake

// Dirs

let outDir = "./output"
let srcDir = outDir + "/src"
let testDir = outDir + "/test"

// Clean

Target "Clean" (fun _ ->
    CleanDirs [ outDir ])

// Restore Packages

Target "Restore" (fun _ ->
    RestorePackages ())

// Build

Target "Build" (fun _ ->
    !! "src/**/*.fsproj"
    |> MSBuildRelease srcDir "Build" 
    |> Log "Build Source: "
    
    !! "test/**/*.fsproj"
    |> MSBuildRelease testDir "Build" 
    |> Log "Build Test: ")

// Test

Target "Test" (fun _ ->
    !! (testDir + "/**/*.Tests.dll")
    |> xUnit (fun p -> 
        { p with HtmlOutput = true
                 OutputDir = testDir }))

// Publish

Target "Publish" (fun _ ->
    NuGet (fun p ->
        { p with
              Authors = [ "Andrew Cherry" ]
              Project = "SemVer.Net"
              OutputPath = outDir
              WorkingDir = srcDir
              Version = "1.0.0"
              AccessKey = getBuildParamOrDefault "nuget_key" ""
              Publish = hasBuildParam "nuget_key"
              Dependencies = [ "FParsec", GetPackageVersion "./packages/" "FParsec" ]
              Files = [ "SemVer.dll", Some "lib/net45", None ] })
              "./nuget/SemVer.nuspec")

// Dependencies

"Clean"
    ==> "Restore"
    ==> "Build"
    ==> "Test"
    ==> "Publish"

RunTargetOrDefault "Test"
