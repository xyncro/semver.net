# SemVer.Net

[![Build status](https://ci.appveyor.com/api/projects/status/kwxecncch8ykjjw8)](https://ci.appveyor.com/project/kolektiv/semver-net)

A SemVer 2.0.0 compliant SemanticVersion Type for .NET...

## Installation

SemVer.Net can be installed from [NuGet](https://www.nuget.org/packages/SemVer.Net "SemVer.Net on NuGet"). Using the Package Manager Console:

```posh
PM> Install-Package SemVer.Net
```

## Usage

Current usage is essentially as simple as a ``ctor`` which takes a string, or static ``Parse`` and ``TryParse`` methods on the type which behave as you would expect them to in .NET languages.

## Rationale

I wanted a genuinely compliant, strict, SemVer 2.0.0 type to use in various places. Existing implementations rely on some slightly lenient regexes, rather than an actual parser - and I like writing parsers. This implementation will choke on everything* it should choke on, and sort everything properly that it should sort. Tests to give some idea of this are provided in the ``test/SemvVer.Tests`` project (specifically, [here](https://github.com/kolektiv/semver.net/blob/master/test/SemVer.Tests/SemVer.Tests.fs "SemVer.Tests.fs on GitHub")).

_*Usual caveats apply when implementing standards. Factual cases to the contrary welcomed - even more welcomed when accompanied by tests and/or fixes as pull requests._
