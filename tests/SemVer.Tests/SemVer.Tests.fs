module SemVer.Tests

open SemVer
open NUnit.Framework
open Swensen.Unquote

// Helpers

type SV = SemanticVersion

// Core Element Validity

[<Test>]
let ``version core elements must be non-negative (SemVer 2.0.0/2)`` () =
    raises<exn> <@ SV "1.1.-1" @>
    raises<exn> <@ SV "1.-1.1" @>
    raises<exn> <@ SV "-1.1.1" @>

[<Test>]
let ``version core elements must not contain leading zeroes (SemVer 2.0.0/2)`` () =
    raises<exn> <@ SV "01.1.1" @>
    raises<exn> <@ SV "1.01.1" @>
    raises<exn> <@ SV "1.1.01" @>

// Pre-Release Validity

[<Test>]
let ``numeric pre-release identifiers must not contain leading zeroes (SemVer 2.0.0/9)`` () =
    raises<exn> <@ SV "1.0.0-01" @>
    raises<exn> <@ SV "1.0.0-alpha.01" @>

[<Test>]
let ``pre-release identifiers must not contain invalid characters (SemVer 2.0.0/9)`` () =
    raises<exn> <@ SV "1.0.0-a.!.c" @>

[<Test>]
let ``pre-release identifiers must not be empty (SemVer 2.0.0/9)`` () =
    raises<exn> <@ SV "1.0.0-a..c" @>

// Build Validity

[<Test>]
let ``build identifiers must not contain invalid characters (SemVer 2.0.0/10)`` () =
    raises<exn> <@ SV "1.0.0+a.!.c" @>

[<Test>]
let ``build identifiers must not be empty (SemVer 2.0.0/10)`` () =
    raises<exn> <@ SV "1.0.0+a..c" @>

// Precedence

[<Test>]
let ``core version exhibits correct (numeric) precedence (SemVer 2.0.0/11)`` () =
    SV "2.1.1" >=? SV "2.1.0"
    SV "2.1.0" >=? SV "2.0.0"
    SV "2.0.0" >=? SV "1.0.0"

[<Test>]
let ``pre-release versions have lower precedence (SemVer 2.0.0/9,11)`` () =
    SV "1.0.0" >=? SV "1.0.0-alpha"

[<Test>]
let ``larger pre-release identifiers have higher precedence (SemVer 2.0.0/11)`` () =
    SV "1.0.0-alpha" <=? SV "1.0.0-alpha.1"

[<Test>]
let ``alpha pre-release identifiers have higher precedence than numeric (SemVer 2.0.0/11)`` () =
    SV "1.0.0-alpha.1" <=? SV "1.0.0-alpha.beta"

[<Test>]
let ``earlier pre-release identifiers have higher precedence (SemVer 2.0.0/11)`` () =
    SV "1.0.0-alpha.beta" <=? SV "1.0.0-beta"

[<Test>]
let ``numeric pre-release identifiers exhibit correct (numeric) precedence (SemVer 2.0.0/11)`` () =
    SV "1.0.0-beta.2" <=? SV "1.0.0-beta.11"
    
[<Test>]
let ``build is ignored for precedence (equality) (SemVer 2.0.0/10)`` () =
    SV "1.0.0+001" =? SV "1.0.0+20130313144700"
    SV "1.0.0+20130313144700" =? SV "1.0.0+exp.sha.5114f85"

// String Conversion

[<Test>]
let ``ToString returns equivalent semantic version string as used for construction`` () =
    string (SV "1.2.3-a.12.c+build.13123") =? "1.2.3-a.12.c+build.13123"

// Convenience Methods

[<Test>]
let ``TryParse returns true for valid semantic version string`` () =
    fst (SemanticVersion.TryParse "1.0.0") =? true

[<Test>]
let ``TryParse returns false for valid semantic version string`` () =
    fst (SemanticVersion.TryParse "1.0") =? false
