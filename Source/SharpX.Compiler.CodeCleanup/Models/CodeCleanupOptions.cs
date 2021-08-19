using System.Text.Json;

using SharpX.Compiler.Extensions;

namespace SharpX.Compiler.CodeCleanup.Models
{
    internal class CodeCleanupOptions
    {
        public AllowList AllowList { get; set; }

        public static CodeCleanupOptions Create(JsonElement element)
        {
            return element.ToObject<CodeCleanupOptions>() ?? new CodeCleanupOptions();
        }
    }
}