namespace SemVer

open System
open System.Runtime.InteropServices


type SemanticVersion (str: string) = 

    member val internal SemVer: SemVer = parseErr str with get, set

    member x.Major with get () = x.SemVer.Version.Major
    member x.Minor with get () = x.SemVer.Version.Minor
    member x.Patch with get () = x.SemVer.Version.Patch

    interface IComparable<SemanticVersion> with

        member x.CompareTo y =
            match Object.ReferenceEquals (y, null) with
            | true -> 1
            | _ -> compare x.SemVer y.SemVer
             
    interface IComparable with

        member x.CompareTo obj =
            match obj with
            | :? SemanticVersion as y -> (x :> IComparable<_>).CompareTo y
            | _ -> invalidArg "obj" "not a SemanticVersion"

    interface IEquatable<SemanticVersion> with

        member x.Equals other =
            (x :> IComparable<_>).CompareTo other = 0        

    override x.Equals obj =
        match obj with
        | :? SemanticVersion as y -> (x :> IEquatable<_>).Equals y
        | _ -> invalidArg "obj" "not a SemanticVersion"

    override x.GetHashCode () =
        hash x.SemVer

    override x.ToString () =
        toString x.SemVer

    new () =
        SemanticVersion ("0.0.0")
    
    static member Parse (str: string) =
        SemanticVersion (str)

    static member TryParse (str: string, [<Out>] success : byref<bool>) =
        match parse str with
        | Choice1Of2 x -> 
            success <- true
            SemanticVersion (SemVer = x)
        | _ ->
            success <- false
            SemanticVersion ()
    