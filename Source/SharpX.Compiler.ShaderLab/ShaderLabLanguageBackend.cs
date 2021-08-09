using SharpX.Compiler.Composition.Attributes;
using SharpX.Compiler.Composition.Enums;
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

            context.RegisterPreSyntaxAction(WellKnownSyntax.ClassDeclarationSyntax, ClassLikeDeclarationWalker.PreProcessClassLikeDeclarations, ClassLikeDeclarationWalker.IsProcessingActionContext);
            context.RegisterPreSyntaxAction(WellKnownSyntax.InterfaceDeclarationSyntax, ClassLikeDeclarationWalker.PreProcessClassLikeDeclarations, ClassLikeDeclarationWalker.IsProcessingActionContext);
            context.RegisterPreSyntaxAction(WellKnownSyntax.RecordDeclarationSyntax, ClassLikeDeclarationWalker.PreProcessClassLikeDeclarations, ClassLikeDeclarationWalker.IsProcessingActionContext);
            context.RegisterPreSyntaxAction(WellKnownSyntax.StructDeclarationSyntax, ClassLikeDeclarationWalker.PreProcessClassLikeDeclarations, ClassLikeDeclarationWalker.IsProcessingActionContext);
            context.RegisterPostSyntaxAction(WellKnownSyntax.ClassDeclarationSyntax, ClassLikeDeclarationWalker.PostProcessClassLikeDeclarations, ClassLikeDeclarationWalker.IsProcessingActionContext);
            context.RegisterPostSyntaxAction(WellKnownSyntax.InterfaceDeclarationSyntax, ClassLikeDeclarationWalker.PostProcessClassLikeDeclarations, ClassLikeDeclarationWalker.IsProcessingActionContext);
            context.RegisterPostSyntaxAction(WellKnownSyntax.RecordDeclarationSyntax, ClassLikeDeclarationWalker.PostProcessClassLikeDeclarations, ClassLikeDeclarationWalker.IsProcessingActionContext);
            context.RegisterPostSyntaxAction(WellKnownSyntax.StructDeclarationSyntax, ClassLikeDeclarationWalker.PostProcessClassLikeDeclarations, ClassLikeDeclarationWalker.IsProcessingActionContext);
            context.RegisterPreSyntaxAction(WellKnownSyntax.PropertyDeclarationSyntax, FieldLikeDeclarationWalker.ProcessPropertyDeclarations, FieldLikeDeclarationWalker.ShouldProcessFieldLikeDeclarations);
            context.RegisterPreSyntaxAction(WellKnownSyntax.FieldDeclarationSyntax, FieldLikeDeclarationWalker.ProcessFieldDeclarations, FieldLikeDeclarationWalker.ShouldProcessFieldLikeDeclarations);
        }

        private void RegisterComponentsForShader(ILanguageBackendContext context)
        {
            context.RegisterExtensionFor(typeof(IShader), "shader");
            context.RegisterSourceContextGeneratorFor(typeof(IShader), _ => new ShaderLabSourceContext());
        }
    }
}