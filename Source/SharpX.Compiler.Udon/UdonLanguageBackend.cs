using System.IO;
using System.Reflection;
using System.Text.Json;

using SharpX.Compiler.Composition.Attributes;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.Udon.Models;

namespace SharpX.Compiler.Udon
{
    [LanguageBackend]
    public class UdonLanguageBackend : ILanguageBackend
    {
        private ExtraCompilerOptions? _options;

        public string Identifier => "Udon";

        public void Initialize(ILanguageBackendContext context)
        {
            ParseCompilerOptions(context.ExtraOptions);
            RegisterComponentsForUdon(context);
        }

        private void ParseCompilerOptions(JsonElement element)
        {
            _options = ExtraCompilerOptions.Create(element);

            foreach (var assembly in Directory.GetFiles(_options.UnityResolver, "*.dll", SearchOption.AllDirectories))
                Assembly.LoadFrom(assembly);

            foreach (var assembly in Directory.GetFiles(_options.VRChatResolver, "*.dll"))
                Assembly.LoadFrom(assembly);

            foreach (var assembly in Directory.GetFiles(_options.UdonResolver, "*.dll"))
                Assembly.LoadFrom(assembly);

            foreach (var assembly in Directory.GetFiles(_options.UdonEditorResolver, "*.dll"))
                Assembly.LoadFrom(assembly);

            foreach (var assembly in Directory.GetFiles(_options.ExternalsResolver, "*.dll"))
                Assembly.LoadFrom(assembly);
        }

        private void RegisterComponentsForUdon(ILanguageBackendContext context)
        {
            context.RegisterExtension("uasm");
            context.RegisterSourceContextFileMappingGenerator(w => Path.Combine("SerializedUdonPrograms", w.OriginalName));
            context.RegisterSourceContextGenerator(_ => new UdonSourceContext());
            context.RegisterCSharpSyntaxWalker(w => new UdonCSharpSyntaxWalker(w));
        }
    }
}