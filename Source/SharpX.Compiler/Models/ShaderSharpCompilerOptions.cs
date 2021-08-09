using System.Collections.Immutable;

namespace SharpX.Compiler.Models
{
    public record SharpXCompilerOptions(ImmutableArray<string> Items, ImmutableArray<string> References, ImmutableArray<string> Plugins, string OutputDir, string Target);
}