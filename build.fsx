#I "packages/FAKE/tools"
#r "packages/FAKE/tools/FakeLib.dll"

open Fake

// Paths

let srcDir = "./output/src"
let testDir = "./output/test"

// Clean

Target "Clean" (fun _ ->
    CleanDirs [ "./output" ])

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
        { p with XmlOutput = true
                 HtmlOutput = true
                 OutputDir = testDir }))

// Dependencies

"Clean"
    ==> "Build"
    ==> "Test"

RunTargetOrDefault "Test"