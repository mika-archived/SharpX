using SharpX.Compiler.Composition.Attributes;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.ShaderLab.Models.HLSL;
using SharpX.Compiler.ShaderLab.Models.Shader;
using SharpX.Library.ShaderLab.Interfaces;

namespace SharpX.Compiler.ShaderLab
{
    [LanguageBackend]
    public class ShaderLabLanguageBackend : ILanguageBackend
    {
        public string Identifier => "ShaderLab";

        public void Initialize(ILanguageBackendContext context)
        {
            RegisterComponentsForShaderLabHLSL(context);
            RegisterComponentsForShader(context);
        }

        private void RegisterComponentsForShaderLabHLSL(ILanguageBackendContext context)
        {
            context.RegisterExtension("cginc");
            context.RegisterSourceContextGenerator(_ => new ShaderLabHLSLSourceContext());
            context.RegisterCSharpSyntaxWalker(w => new ShaderLabCSharpSyntaxWalker(w));
        }

        private void RegisterComponentsForShader(ILanguageBackendContext context)
        {
            context.RegisterExtensionFor(typeof(IShader), "shader");
            context.RegisterSourceContextGeneratorFor(typeof(IShader), _ => new ShaderLabSourceContext());
        }
    }
}