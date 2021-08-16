using System.Collections.Immutable;
using System.Text.Json;

using SharpX.Compiler.Extensions;

namespace SharpX.Compiler.ShaderLab.Models
{
    public class ExtraCompilerOptions
    {
        public ImmutableArray<ShaderVariant> ShaderVariants { get; init; }

        public static ExtraCompilerOptions Create(JsonElement element)
        {
            return element.ToObject<ExtraCompilerOptions>() ?? new ExtraCompilerOptions();
        }
    }
}