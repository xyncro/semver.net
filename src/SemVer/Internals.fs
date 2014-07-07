namespace SemVer

open System


[<AutoOpen>]
module internal Types =

    type SemVer =
        { Version: Version
          PreRelease: PreRelease option
          Build: Build option }

        static member make (version, preRelease, build) =
            { Version = version
              PreRelease = preRelease
              Build = build }

    and Version =
        { Major: uint32
          Minor: uint32
          Patch: uint32 }

        static member make (major, minor, patch) =
            { Major = major
              Minor = minor
              Patch = patch }

    and PreRelease =
        PreReleaseComp seq

    and PreReleaseComp =
        | Alpha of string
        | Numeric of uint32

    and Build =
        string seq


[<AutoOpen>]
module internal Parser =

    open FParsec

    // Convenience Operators

    let (>!>.) p1 p2 = notFollowedBy p1 >>. p2
    let (!!!) p1 = attempt p1
    let (!!<) p1 = lookAhead p1

    // Standard Tokens

    let dot = pchar '.'
    let dash = pchar '-'
    let plus = pchar '+'

    let eof = eof |>> fun _ -> EOS

    // Numeric Identifiers
    
    let zero = pchar '0' |>> fun _ -> 0u
    let nonZero = zero >!>. puint32
    let numeric = zero <|> nonZero 

    // Textual/Alphanumeric Identifiers

    let conc: char seq -> string = String.Concat
    let cons: string seq -> string = String.Concat

    let text = many1 (choice [ digit; asciiLetter; dash ]) |>> conc

    let zeroStr (a, b, c) = cons [ conc a; string b; c |> function | Some c -> c | _ -> "" ]
    let zeroLead = tuple3 (many digit) (choice [ asciiLetter; dash ]) (opt text) |>> zeroStr
    let nonZeroLead = zero >!>. text
    let alpha = zeroLead <|> nonZeroLead
    
    // Core Version

    let ma, mi, p = numeric, numeric, numeric
    let core = tuple3 (ma .>> dot) (mi .>> dot) (p .>> (choice [ !!< dash; !!< plus; eof ])) |>> Version.make

    // Pre-Release

    let pri t = (!!! (numeric .>> t |>> Numeric) <|> (alpha .>> t |>> Alpha))
    let pris = many (!!! (pri dot)) .>>. (pri (choice [ !!< plus; eof ])) |>> fun (x, y) -> x @ [y]
    let pr = dash >>. pris |>> Seq.ofList

    // Build

    let bis = many (!!! (text .>> dot)) .>>. (text .>> eof) |>> fun (x, y) -> x @ [y]
    let b = plus >>. bis |>> Seq.ofList

    // Semantic Version

    let sv = tuple3 core (opt pr) (opt b) |>> SemVer.make

    // Throwing/Non-Throwing Parse Functions

    let parse str =
        match run sv str with
        | Success (x, _, _) -> Choice1Of2 x
        | Failure (err, _, _) -> Choice2Of2 err

    let parseErr str =
        match parse str with
        | Choice1Of2 x -> x
        | Choice2Of2 err -> failwith err


[<AutoOpen>]
module internal Compare =

    // Compare Core Version Specs

    let compareVersion x y =
        let toList x =
            [ x.Major; x.Minor; x.Patch ]

        Seq.zip (toList x) (toList y)
        |> Seq.tryFind (fun (x, y) -> x <> y)
        |> function | Some (x, y) -> x.CompareTo y | _ -> 0

    // Compare Pre-Release Specs (Components > Component Arity)
    
    let comparePreRelease x y =
        let compare x y =
            match x, y with
            | Alpha x, Alpha y -> x.CompareTo y
            | Numeric x, Numeric y -> x.CompareTo y
            | Alpha x, _ -> 1
            | _, _ -> -1

        match x, y with
        | Some x, Some y ->
            Seq.zip x y
            |> Seq.tryFind (fun (x, y) -> x <> y)
            |> function | Some (x, y) -> compare x y | _ -> 0
            |> function | 0 -> (Seq.length x).CompareTo (Seq.length y) | x -> x
        | None, Some _ -> 1
        | Some _, None -> -1
        | _, _ -> 0

    // Compare Semantic Versions (Core > Pre-Release)

    let compare x y =
        match compareVersion x.Version y.Version with
        | 0 -> comparePreRelease x.PreRelease y.PreRelease
        | x -> x


[<AutoOpen>]
module internal Hash =

    let hash (x: SemVer) =
        hash x


[<AutoOpen>]
module internal ToString =

    let versionToS (x: Version) =
        sprintf "%d.%d.%d" x.Major x.Minor x.Patch

    let preReleaseToS (x: PreRelease option) =
        match x with
        | Some x -> sprintf "-%s" (String.concat "." (Seq.map (function | Alpha x -> x | Numeric x -> string x) x))
        | _ -> ""

    let buildToS (x: Build option) =
        match x with
        | Some x -> sprintf "+%s" (String.concat "." x)
        | _ -> ""

    let toString (x: SemVer) =
        sprintf "%s%s%s" (versionToS x.Version) (preReleaseToS x.PreRelease) (buildToS x.Build)
