#!/usr/bin/env dotnet-script

#nullable enable

// #r "nuget: System.Text.RegularExpressions"

using System.Buffers;
using System.Threading;

public enum RepositoryType {
    File,
    JsonFile,
    YamlFile,
    Sqlite,
    SqlServer,
    PostgreSQL,
    MySql,
}

public class SemanticVersionEventArgs {
    public SemanticVersion Version { get; }

    public SemanticVersionEventArgs(SemanticVersion version) {
        this.Version = version ?? throw new ArgumentNullException(nameof(version));
    }
}

public class SemanticVersion  {
    public string? Prefix { get; internal set; }
    public string Major { get; internal set; } = "1";
    public string? Minor { get; internal set; }
    public string? Patch { get; internal set; }
    public string? Postfix { get; internal set; } = string.Empty;
    public bool? IsPrerelease { get; internal set; } = false;

    public override string ToString() => new StringBuilder()
        .Append(this.Prefix)
        .Append(this.Major)
        .Append(".")
        .Append(this.Minor)
        .Append(".")
        .Append(this.Patch)
        .Append("-")
        .Append(this.Postfix)
        .ToString();
}

public interface IVersionRepository {
    EventHandler<SemanticVersionEventArgs> VersionAdded { get; }
    EventHandler<SemanticVersionEventArgs> MajorUpdated { get; }
    EventHandler<SemanticVersionEventArgs> MinorUpdated { get; }
    EventHandler<SemanticVersionEventArgs> PatchUpdated { get; }
    EventHandler<SemanticVersionEventArgs> PostfixUpdated { get; }

    ValueTask<SemanticVersion> Add(SemanticVersion semver);
    ValueTask<SemanticVersion> Add(string major, string? minor, string? patch, bool? prerelease = null);
    ValueTask<SemanticVersion> Get(string major, string? minor, string? patch, bool? prerelease = null);
    ValueTask<SemanticVersion> GetLatest(bool? prerelease = false);
    ValueTask<SemanticVersion> Set(string major, string? minor, string? patch, bool? prerelease = null);
}

public sealed class DotEnvSemver {
    public string Path { get; }
    public SemanticVersion Version { get; }

    private DotEnvSemver(string path, SemanticVersion version) {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (!File.Exists(path))
            throw new FileNotFoundException("File not found.", path);

        this.Path = path;
        this.Version = version;
    }

    public static async Task<DotEnvSemver> ReadVersion(string path, CancellationToken cancellationToken = default) {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (!File.Exists(path))
            throw new FileNotFoundException("File not found.", path);

        var contents = await File.ReadAllLinesAsync(path, cancellationToken);
        var version = contents
            .Where(x => !x.StartsWith('#'))
            .Select(x => {
                var commentStartIdx = x.IndexOf('#', StringComparison.OrdinalIgnoreCase);
                if (commentStartIdx == -1)
                    return x;
                else
                    return x.Substring(0, commentStartIdx + 1);
            })
            .Select(x => {
                var semantics = x.Split('.');
                if (semantics.Length < 3)
                    throw new IndexOutOfRangeException("Invalid semver");

                var dotSplitLeft = semantics[0];
                var dotSplitMiddle = semantics[1];
                var dotSplitRight = semantics[2];

                var prefix = dotSplitLeft.TakeWhile(c => char.IsLetter(c))?.ToString();
                var major = semantics[0].Skip(prefix?.Length ?? 0)?.ToString() ?? "1";
                var minor = semantics[1] ?? "0";
                var patch = semantics[2]?.TakeWhile(c => char.IsDigit(c))?.ToString() ?? "0";
                var postfix = (semantics[2]?.Any(c => !char.IsDigit(c)) ?? false) ?
                    semantics[2].Split(semantics[2].First(c => !char.IsDigit(c))).FirstOrDefault() :
                    null;

                return new SemanticVersion() {
                    Prefix = prefix,
                    Major = major,
                    Minor = minor,
                    Patch = patch,
                    Postfix = postfix
                };
            }).LastOrDefault();

        var prereleaseIndicators = new[] { "alpha", "beta", "rc", "prerelease" };
        foreach (var indicator in prereleaseIndicators) {
            if (version?.Postfix?.Contains(indicator) ?? false)
                version.IsPrerelease = true;
        }

        return new DotEnvSemver(path, version);
    }

}
