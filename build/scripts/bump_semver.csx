#nullable enable

#r "nuget: System.Text.Json, 6.0.0"
#r "nuget: System.Text.Json.Serialization, 6.0.0"

namespace SemverScript.Bump;

using Microsoft.VisualBasic.CompilerServices;

using System;
using System.Buffers;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Runtime.CompilerServices;

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
    private Regex regex = new Regex("[^a-zA-Z]");

    public string Prefix { get; internal set; } = "v";
    public string Major { get; internal set; } = "1";
    public string? Minor { get; internal set; }
    public string? Patch { get; internal set; }
    public string Postfix { get; internal set; } = string.Empty;
    public bool? IsPrerelease => !string.IsNullOrWhiteSpace(this.Postfix) && new string[] { "alpha", "beta", "rc", "prerelease", "preview", "pr" }.Contains(this.regex.Replace(this.Postfix.ToLowerInvariant(), string.Empty);

    public operator string() {
        var sb = new StringBuilder();
        sb.Append(this.Prefix);
        sb.Append(this.Major);
        if (!string.IsNullOrWhiteSpace(this.Minor)) {
            sb.Append('.');
            sb.Append(this.Minor);
        }
        if (!string.IsNullOrWhiteSpace(this.Patch)) {
            sb.Append('.');
        }
    }

    private string PostfixToString() {
        $"{(string.IsNullOrWhiteSpace(this.Postfix) ?
            string.Empty :
            $"-{this.Postfix}";
    }

    public override string ToString() => $"{this.Prefix}{this.Major}.{this.Minor}.{this.Patch}" +

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

public class VersionPerFile {
    public string Path { get; }
    public SemanticVersion Version { get; }

    private VersionPerFile(string path, SemanticVersion version) {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (!File.Exists(path))
            throw new FileNotFoundException("File not found.", path);

        this.Path = path;
        this.Version = version ;
    }

    public static async Task<SemanticVersion> ReadVersion(string path, CancellationToken cancellationToken = default) {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (!File.Exists(path))
            throw new FileNotFoundException("File not found.", path);

        var contents = await File.ReadAllLinesAsync(path, cancellationToken);
        contents
            .Where(x => !x.StartsWith('#'))
            .Select(x => {
                var commentStartIdx = x.IndexOf('#', StringComparison.OrdinalIgnoreCase);
                if (commentStartIdx == -1)
                    return x;
                else
                    return x.Substring(0, commentStartIdx + 1);
            })
            .Select(x => {
                var semantics = x.Split('.', StringComparison.OrdinalIgnoreCase);
                var prefix =
            });
    }
}
