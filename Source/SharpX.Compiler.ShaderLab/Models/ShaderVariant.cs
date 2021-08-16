using System.Collections.Immutable;

namespace SharpX.Compiler.ShaderLab.Models
{
    public class ShaderVariant
    {
        public string Name { get; init; }

        public ImmutableArray<string> Directives { get; init; }
    }
}