using System.Collections.Immutable;
using System.IO;
using System.Linq;

using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

using ShaderSharp.Compiler.Models;

namespace ShaderSharp.CLI.Models
{
    public record CompilerConfiguration
    {
        public string[] Sources { get; set; }

        public string[] References { get; set; }

        public string[] Plugins { get; set; }

        public string Out { get; set; }

        public ShaderSharpCompilerOptions ToCompilerOptions()
        {
            var matcher = new Matcher();
            matcher.AddIncludePatterns(Sources);

            var items = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(Directory.GetCurrentDirectory())));

            return new ShaderSharpCompilerOptions
            {
                Items = items.Files.Select(w => w.Path).ToImmutableArray(),
                References = References.ToImmutableArray(),
                Plugins = Plugins.ToImmutableArray(),
                OutputDir = Out
            };
        }
    }
}