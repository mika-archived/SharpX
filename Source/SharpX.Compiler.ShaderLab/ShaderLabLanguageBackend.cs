using System.IO;
using System.Linq;
using System.Text.Json;

using SharpX.Compiler.Composition.Attributes;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.ShaderLab.Models;
using SharpX.Compiler.ShaderLab.Models.HLSL;

namespace SharpX.Compiler.ShaderLab
{
    [LanguageBackend]
    public class ShaderLabLanguageBackend : ILanguageBackend
    {
        private ExtraCompilerOptions? _options;

        public string Identifier => "ShaderLab";

        public void Initialize(ILanguageBackendContext context)
        {
            ParseCompilerOptions(context.ExtraOptions);
            RegisterComponentsForShaderLabHLSL(context);
        }

        private void ParseCompilerOptions(JsonElement element)
        {
            _options = ExtraCompilerOptions.Create(element);
        }

        // ReSharper disable once InconsistentNaming
        private void RegisterComponentsForShaderLabHLSL(ILanguageBackendContext context)
        {
            context.RegisterExtension("cginc");
            context.RegisterSourceContextFileMappingGenerator(w => Path.Combine(w.Variant, w.OriginalName));
            context.RegisterSourceContextGenerator(_ => new ShaderLabHLSLSourceContext());
            context.RegisterCSharpSyntaxWalker(w => new ShaderLabCSharpSyntaxWalker(w));

            foreach (var variant in _options!.ShaderVariants)
                context.RegisterCompilationVariants(variant.Name, variant.Directives.ToArray());
        }
    }
}