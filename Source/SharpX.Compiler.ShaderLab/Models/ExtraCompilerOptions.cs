using System.Collections.Immutable;
using System.Text.Json;

using SharpX.Compiler.Extensions;

namespace SharpX.Compiler.ShaderLab.Models
{
    public class ExtraCompilerOptions
    {
        public ImmutableArray<ShaderVariant>? ShaderVariants { get; set; }

        public static ExtraCompilerOptions Create(JsonElement element)
        {
            var obj= element.ToObject<ExtraCompilerOptions>() ?? new ExtraCompilerOptions();
            if (obj.ShaderVariants == null)
                obj.ShaderVariants = ImmutableArray<ShaderVariant>.Empty;

            return obj;
        }
    }
}