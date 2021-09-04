using System.Collections.Immutable;
using System.Text.Json;

namespace SharpX.Compiler.Models
{
    public record SharpXCompilerOptions(string ProjectRoot, ImmutableArray<string> References, ImmutableArray<string> Plugins, string OutputDir, string Target, ImmutableDictionary<string, JsonElement> CustomOptions);
}