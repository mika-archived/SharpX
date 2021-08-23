using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

using SharpX.Compiler.Models;

namespace SharpX.CLI.Models
{
    public record CompilerConfiguration(string BaseDir, string[] Sources, string[] References, string[] Plugins, string Out, string Target)
    {
        [JsonExtensionData]
        public Dictionary<string, JsonElement> CustomOptions { get; init; } = new();

        public SharpXCompilerOptions ToCompilerOptions()
        {
            var matcher = new Matcher();
            matcher.AddIncludePatterns(Sources);

            var items = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(BaseDir)));

            var options = new SharpXCompilerOptions(items.Files.Select(w => w.Path).ToImmutableArray(), References.ToImmutableArray(), Plugins.ToImmutableArray(), Out, Target, CustomOptions.ToImmutableDictionary());
            return options with { ProjectRoot = BaseDir };
        }
    }
}