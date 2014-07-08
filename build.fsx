#I "packages/FAKE/tools"
#r "packages/FAKE/tools/FakeLib.dll"

open Fake

// Build

Target "Build" (fun _ ->
    !! "src/**/*.fsproj" 
    ++ "test/**/*.fsproj"
    |> MSBuildRelease "" "Build" 
    |> Log "Build: ")

// Test

Target "Test" (fun _ ->
    !! "test/**/bin/Release/*.Tests.dll"
    |> xUnit (fun p -> 
        { p with XmlOutput = true
                 HtmlOutput = true }))

// Dependencies

"Build"
    ==> "Test"

RunTargetOrDefault "Test"