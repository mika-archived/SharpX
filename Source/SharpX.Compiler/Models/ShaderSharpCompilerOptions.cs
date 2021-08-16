using System.Collections.Immutable;
using System.Text.Json;

namespace SharpX.Compiler.Models
{
    public record SharpXCompilerOptions(ImmutableArray<string> Items, ImmutableArray<string> References, ImmutableArray<string> Plugins, string OutputDir, string Target, ImmutableDictionary<string, JsonElement> CustomOptions);
}