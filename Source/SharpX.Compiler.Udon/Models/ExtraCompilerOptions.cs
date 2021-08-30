using System.Text.Json;

using SharpX.Compiler.Extensions;

namespace SharpX.Compiler.Udon.Models
{
    internal class ExtraCompilerOptions
    {
        public string UnityResolver { get; init; }

        public string VRChatResolver { get; init; }

        public string UdonResolver { get; init; }

        public string UdonEditorResolver { get; init; }

        public string ExternalsResolver { get; init; }

        public static ExtraCompilerOptions Create(JsonElement element)
        {
            var obj = element.ToObject<ExtraCompilerOptions>() ?? new ExtraCompilerOptions();
            return obj;
        }
    }
}