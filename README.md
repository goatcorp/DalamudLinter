# DalamudLinter

DalamudLinter is a NuGet package that can be added to Dalamud plugin
projects to check for common issues and mistakes. It produces warnings
and errors before building if necessary.

## Usage

Install the NuGet package `DalamudLinter`. Build in the release
configuration.

## Advanced usage

`DalamudLinter` will also read from a `DalamudLinter.targets` file. An
example is given below.

```xml
<Project>
    <Target Name="DalamudLinter" BeforeTargets="Build" Condition="'$(Configuration)' == 'Release'">
        <DalamudLinter 
                ProjectDir="$(ProjectDir)"
                ProjectFilePath="$(MSBuildProjectFullPath)"/>
    </Target>
</Project>
```

## Ignoring lints

Specify an `IgnoredLints` property in a `PropertyGroup` tag in the
project's `csproj` file. Lints may be separated by a semicolon (`;`),
newline, or any combination of the two.

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <!-- ... -->
        <IgnoredLints>
            D0001; D0002
            D0003
        </IgnoredLints>
        <!-- ... -->
    </PropertyGroup>
</Project>
```

## Lints

Lints are specified in a `Resources/lints.yaml`. A copy of this file
is embedded into each release of `DalamudLinter`.

See the `Model/Lint.cs` file for documentation about each lint
parameter.
