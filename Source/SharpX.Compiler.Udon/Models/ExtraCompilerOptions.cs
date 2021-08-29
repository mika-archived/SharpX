using System.Text.Json;

using SharpX.Compiler.Extensions;

namespace SharpX.Compiler.Udon.Models
{
    internal class ExtraCompilerOptions
    {
        public static ExtraCompilerOptions Create(JsonElement element)
        {
            var obj = element.ToObject<ExtraCompilerOptions>() ?? new ExtraCompilerOptions();

            return obj;
        }
    }
}