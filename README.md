# PrereleaseAttributes
Proposed prerelease attributes to signify members/types that are prerelease.

PrereleaseAttributes has 4 attriutes: `PrereleaseAttribute`, `ExperimentalAttribute`, `AlphaAttribute`, and `PreviewAttribute`.  The idea is that someone writing code or providing a library to someone else could add one of these attributes to signify the code is one or more of instable, unsupported, or untested.

The idea is that an IDE/Analyzer could detect that attribute and warn/error that a code base was using experimental features.
## Usage

Pretty straightforward:
```csharp
[Experimental]
public class MyExperiment
{
    // omitted
}
```

Ideally, but optionally, you can include detail about *why*:
```csharp
[Experimental("I just wrote this one day, only 33% code coverage.")]
public class MyExperiment
{
    // omitted
}
```

Icons made by [Freepik](http://www.freepik.com) from [www.flatiron.com](http://www.flaticon.com/ "Flaticon") is licensed by [CC 3.0 BY](http://creativecommons.org/licenses/by/3.0/" "Creative Commons BY 3.0")