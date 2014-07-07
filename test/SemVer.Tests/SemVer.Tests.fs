module SemVer.Tests

open SemVer
open Swensen.Unquote
open Xunit

// Helpers

type SV = SemanticVersion

// Core Element Validity

[<Fact>]
let ``version core elements must be non-negative (SemVer 2.0.0/2)`` () =
    raises<exn> <@ SV "1.1.-1" @>
    raises<exn> <@ SV "1.-1.1" @>
    raises<exn> <@ SV "-1.1.1" @>

[<Fact>]
let ``version core elements must not contain leading zeroes (SemVer 2.0.0/2)`` () =
    raises<exn> <@ SV "01.1.1" @>
    raises<exn> <@ SV "1.01.1" @>
    raises<exn> <@ SV "1.1.01" @>

// Pre-Release Validity

[<Fact>]
let ``numeric pre-release identifiers must not contain leading zeroes (SemVer 2.0.0/9)`` () =
    raises<exn> <@ SV "1.0.0-01" @>
    raises<exn> <@ SV "1.0.0-alpha.01" @>

[<Fact>]
let ``pre-release identifiers must not contain invalid characters (SemVer 2.0.0/9)`` () =
    raises<exn> <@ SV "1.0.0-a.!.c" @>

[<Fact>]
let ``pre-release identifiers must not be empty (SemVer 2.0.0/9)`` () =
    raises<exn> <@ SV "1.0.0-a..c" @>

// Build Validity

[<Fact>]
let ``build identifiers must not contain invalid characters (SemVer 2.0.0/10)`` () =
    raises<exn> <@ SV "1.0.0+a.!.c" @>

[<Fact>]
let ``build identifiers must not be empty (SemVer 2.0.0/10)`` () =
    raises<exn> <@ SV "1.0.0+a..c" @>

// Precedence

[<Fact>]
let ``core version exhibits correct (numeric) precedence (SemVer 2.0.0/11)`` () =
    SV "2.1.1" >=? SV "2.1.0"
    SV "2.1.0" >=? SV "2.0.0"
    SV "2.0.0" >=? SV "1.0.0"

[<Fact>]
let ``pre-release versions have lower precedence (SemVer 2.0.0/9,11)`` () =
    SV "1.0.0" >=? SV "1.0.0-alpha"

[<Fact>]
let ``larger pre-release identifiers have higher precedence (SemVer 2.0.0/11)`` () =
    SV "1.0.0-alpha" <=? SV "1.0.0-alpha.1"

[<Fact>]
let ``alpha pre-release identifiers have higher precedence than numeric (SemVer 2.0.0/11)`` () =
    SV "1.0.0-alpha.1" <=? SV "1.0.0-alpha.beta"

[<Fact>]
let ``earlier pre-release identifiers have higher precedence (SemVer 2.0.0/11)`` () =
    SV "1.0.0-alpha.beta" <=? SV "1.0.0-beta"

[<Fact>]
let ``numeric pre-release identifiers exhibit correct (numeric) precedence (SemVer 2.0.0/11)`` () =
    SV "1.0.0-beta.2" <=? SV "1.0.0-beta.11"
    
[<Fact>]
let ``build is ignored for precedence (equality) (SemVer 2.0.0/10)`` () =
    SV "1.0.0+001" =? SV "1.0.0+20130313144700"
    SV "1.0.0+20130313144700" =? SV "1.0.0+exp.sha.5114f85"

// String Conversion

[<Fact>]
let ``ToString returns equivalent semantic version string as used for construction`` () =
    string (SV "1.2.3-a.12.c+build.13123") =? "1.2.3-a.12.c+build.13123"

// Convenience Methods

[<Fact>]
let ``TryParse returns true for valid semantic version string`` () =
    snd (SemanticVersion.TryParse "1.0.0") =? true

[<Fact>]
let ``TryParse returns false for valid semantic version string`` () =
    snd (SemanticVersion.TryParse "1.0") =? false
