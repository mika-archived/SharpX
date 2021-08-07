using System.Collections.Immutable;

namespace ShaderSharp.Compiler.Models
{
    public record ShaderSharpCompilerOptions
    {
        public ImmutableArray<string> Items { get; init; }

        public ImmutableArray<string> References { get; init; }

        public ImmutableArray<string> Plugins { get; init; }

        public string OutputDir { get; init; }
    }
}