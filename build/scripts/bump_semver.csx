#!/usr/bin/env dotnet-script

#nullable enable

#r "System"
#r "System.Text.RegularExpressions"

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public class SemanticVersion
{
    public int Major { get; set; }
    public int Minor { get; set; }
    public int Patch { get; set; }
    public string? Build { get; set; }
    public string? PreRelease { get; set; }
    public string? Metadata { get; set; }

    public SemanticVersion(int major, int minor, int patch, string? preRelease = null, string? build = null, string? metadata = null)
    {
        if (major == 0 && minor == 0 && patch == 0)
        {
            throw new ArgumentException("Version must be greater than 0.0.0");
        }

        Major = major;
        Minor = minor;
        Patch = patch;
        Build = build;
        PreRelease = preRelease;
        Metadata = metadata;
    }

    public SemanticVersion(string version)
    {

    }

    public bool Validate()
    {
        return this.Major != 0 && this.Minor != 0 && this.Patch != 0;
    }

    public override string ToString()
    {

    }
}

public class SemanticVersionSerializer : ISemanticVersionSerializer
{
    public string Serialize(SemanticVersion v)
    {
        var semver = $"{v.Major}.{v.Minor}.{v.Patch}";
        if (v.Build != null)
            semver += $".{v.Build}";
        if (v.PreRelease != null)
            semver += $"-{v.PreRelease}";
        if (v.Metadata != null)
            semver += $"+{v.Metadata}";
        return semver;
    }

    public SemanticVersion Deserialize(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Version must not be null or empty");

        var regex = new Regex(@"^(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(\.(?<build>\d+))?(\-(?<prerelease>\w+))?(\+(?<metadata>\w+))?$");
        var match = regex.Match(version);
        if (!match.Success)
            throw new ArgumentException("Version must be in the format of major.minor.patch[.build][-prerelease][+metadata]");

        try
        {
            var sv = new
            {
                Major = int.Parse(match.Groups["major"].Value),
                Minor = int.Parse(match.Groups["minor"].Value),
                Patch = int.Parse(match.Groups["patch"].Value),
                Build = match.Groups["build"].Value?.Trim(),
                PreRelease = match.Groups["prerelease"].Value?.Trim(),
                Metadata = match.Groups["metadata"].Value?.Trim()
            };

            return new SemanticVersion(
                sv.Major,
                sv.Minor,
                sv.Patch,
                sv.PreRelease, sv.Build, sv.Metadata);
        }
        catch (TypeInitializationException e)
        {
            throw new ArgumentException("major.minor.patch must all be valid integer types.", e);
        }
        catch (Exception e)
        {
            throw new ArgumentException("Version must be in the format of major.minor.patch[.build][-prerelease][+metadata]", e);
        }


        if (!int.TryParse(match.Groups["major"].Value, out var major))
            throw new Exception("Major must be an integer");

        if (!int.TryParse(match.Groups["minor"].Value, out var minor))
            throw new Exception("Minor must be an integer");

        if (!int.TryParse(match.Groups["patch"].Value, out var patch))
            throw new Exception("Patch must be an integer");

        if (match.Groups["build"].Success && !int.TryParse(match.Groups["build"].Value, out var build))
        {
            throw new Exception("Build must be an integer");
        }

        if (match.Groups["prerelease"].Success)
            this.PreRelease = match.Groups["prerelease"].Value;

        if (match.Groups["metadata"].Success)
            this.Metadata = match.Groups["metadata"].Value;
    }
}

public interface ISemanticVersionSerializer
{
    string Serialize(SemanticVersion v);
    SemanticVersion Deserialize(string s);
}

static class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: bump_semver <current version>");
            return;
        }

        var version = args[0];
        var regex = new Regex(@"^(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(\.(?<build>\d+))?(\-(?<prerelease>\w+))?(\+(?<metadata>\w+))?$");
        var match = regex.Match(version);
        if (!match.Success)
        {
            Console.WriteLine("Invalid version");
            return;
        }

        var major = int.Parse(match.Groups["major"].Value);
        var minor = int.Parse(match.Groups["minor"].Value);
        var patch = int.Parse(match.Groups["patch"].Value);
        var build = match.Groups["build"].Success ? int.Parse(match.Groups["build"].Value) : 0;
        var prerelease = match.Groups["prerelease"].Success ? match.Groups["prerelease"].Value : null;
        var metadata = match.Groups["metadata"].Success ? match.Groups["metadata"].Value : null;

        var newVersion = $"{major}.{minor}.{patch}.{build + 1}";
        if (prerelease != null)
        {
            newVersion += $"-{prerelease}";
        }
        if (metadata != null)
        {
            newVersion += $"+{metadata}";
        }
        Console.WriteLine(newVersion);
    }
}

// #r "nuget: System.Text.RegularExpressions"

// #r "System"
// #r "System.Threading"
// #r "System.Threading.Tasks"

// #r "System.Collections.Generic"
// #r "System.Linq"
// #r "System.Text"
// #r "System.Text.RegularExpressions"
// #r "System.IO"
// #r "System.Diagnostics"
// #r "System.Reflection"
// #r "System.Runtime.CompilerServices"
// #r "System.Runtime.InteropServices"
// #r "System.Runtime.Serialization"
// #r "System.Runtime.Serialization.Json"

// using System.Buffers;
// using System.Threading;

// public class SemanticVersion  {
//     public string? Prefix { get; internal set; }
//     public string Major { get; internal set; } = "1";
//     public string? Minor { get; internal set; }
//     public string? Patch { get; internal set; }
//     public string? Postfix { get; internal set; } = string.Empty;
//     public bool? IsPrerelease { get; internal set; } = false;

//     public override string ToString() => new StringBuilder()
//         .Append(this.Prefix)
//         .Append(this.Major)
//         .Append(".")
//         .Append(this.Minor)
//         .Append(".")
//         .Append(this.Patch)
//         .Append("-")
//         .Append(this.Postfix)
//         .ToString();
// }

// public sealed class DotEnvSemver {
//     public string Path { get; }
//     public SemanticVersion Version { get; }

//     private DotEnvSemver(string path, SemanticVersion version) {
//         if (string.IsNullOrWhiteSpace(path))
//             throw new ArgumentNullException(nameof(path));

//         if (!File.Exists(path))
//             throw new FileNotFoundException("File not found.", path);

//         this.Path = path;
//         this.Version = version;
//     }

//     public static async Task<DotEnvSemver> ReadVersion(string path, CancellationToken cancellationToken = default) {
//         if (string.IsNullOrWhiteSpace(path))
//             throw new ArgumentNullException(nameof(path));

//         if (!File.Exists(path))
//             throw new FileNotFoundException("File not found.", path);

//         var contents = await File.ReadAllLinesAsync(path, cancellationToken);
//         var version = contents
//             .Where(x => !x.StartsWith('#'))
//             .Select(x => {
//                 var commentStartIdx = x.IndexOf('#', StringComparison.OrdinalIgnoreCase);
//                 if (commentStartIdx == -1)
//                     return x;
//                 else
//                     return x.Substring(0, commentStartIdx + 1);
//             })
//             .Select(x => {
//                 var semantics = x.Split('.');
//                 if (semantics.Length < 3)
//                     throw new IndexOutOfRangeException("Invalid semver");

//                 var dotSplitLeft = semantics[0];
//                 var dotSplitMiddle = semantics[1];
//                 var dotSplitRight = semantics[2];

//                 var prefix = dotSplitLeft.TakeWhile(c => char.IsLetter(c))?.ToString();
//                 var major = semantics[0].Skip(prefix?.Length ?? 0)?.ToString() ?? "1";
//                 var minor = semantics[1] ?? "0";
//                 var patch = semantics[2]?.TakeWhile(c => char.IsDigit(c))?.ToString() ?? "0";
//                 var postfix = (semantics[2]?.Any(c => !char.IsDigit(c)) ?? false) ?
//                     semantics[2].Split(semantics[2].First(c => !char.IsDigit(c))).FirstOrDefault() :
//                     null;

//                 return new SemanticVersion() {
//                     Prefix = prefix,
//                     Major = major,
//                     Minor = minor,
//                     Patch = patch,
//                     Postfix = postfix
//                 };
//             }).LastOrDefault();

//         var prereleaseIndicators = new[] { "alpha", "beta", "rc", "prerelease" };
//         foreach (var indicator in prereleaseIndicators) {
//             if (version?.Postfix?.Contains(indicator) ?? false)
//                 version.IsPrerelease = true;
//         }

//         return new DotEnvSemver(path, version);
//     }
// }
