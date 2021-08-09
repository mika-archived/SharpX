using System.Collections.Immutable;
using System.IO;
using System.Linq;

using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

using SharpX.Compiler.Models;

namespace SharpX.CLI.Models
{
    public record CompilerConfiguration(string[] Sources, string[] References, string[] Plugins, string Out, string Target)
    {
        public SharpXCompilerOptions ToCompilerOptions()
        {
            var matcher = new Matcher();
            matcher.AddIncludePatterns(Sources);

            var items = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(Directory.GetCurrentDirectory())));

            return new SharpXCompilerOptions
            {
                Items = items.Files.Select(w => w.Path).ToImmutableArray(),
                References = References.ToImmutableArray(),
                Plugins = Plugins.ToImmutableArray(),
                OutputDir = Out,
                Target = Target
            };
        }
    }
}