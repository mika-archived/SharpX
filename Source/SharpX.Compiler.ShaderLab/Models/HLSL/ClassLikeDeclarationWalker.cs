using System.Diagnostics;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL
{
    public static class ClassLikeDeclarationWalker
    {
        public static bool IsProcessingActionContext(ILanguageSyntaxActionContext context)
        {
            var capture = ClassLikeDeclarationCapture.Capture(context.Node, context.SemanticModel);
            if (capture == null)
                return false;
            if (capture.HasInherit<IShader>())
                return false;
            return true;
        }

        public static void PreProcessClassLikeDeclarations(ILanguageSyntaxActionContext context)
        {
            var capture = ClassLikeDeclarationCapture.Capture(context.Node, context.SemanticModel);
            if (capture == null)
            {
                context.Errors.Add(new DefaultError(context.Node, "Failed to load class-like syntax structure"));
                context.StopPropagation();
                return;
            }

            if (capture.IsNested())
            {
                context.Errors.Add(new DefaultError(context.Node, "SharpX ShaderLab Compiler does not support nested type declarations"));
                context.StopPropagation();
                return;
            }

            if (capture.HasAttribute<ExternalAttribute>())
                return;

            if (capture.HasAttribute<ExportAttribute>())
            {
                var attr = capture.GetAttribute<ExportAttribute>()!;
                if (!attr.IsValidSourceName())
                {
                    context.Errors.Add(new DefaultError(context.Node, "The value specified for ComponentAttribute must be a valid file name"));
                    context.StopPropagation();
                    return;
                }

                context.CreateOrGetContext(attr.Source);
            }

            if (capture.HasAttribute<ComponentAttribute>())
            {
                var attr = capture.GetAttribute<ComponentAttribute>()!;
                context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.OpenStruct(string.IsNullOrWhiteSpace(attr.Name) ? capture.GetDeclarationName() : attr.Name);
            }
        }

        public static void PostProcessClassLikeDeclarations(ILanguageSyntaxActionContext context)
        {
            var capture = ClassLikeDeclarationCapture.Capture(context.Node, context.SemanticModel)!; // not-null, because StopPropagated in upstream

            Debug.WriteLine(context.GetType());

            if (capture.HasAttribute<ExportAttribute>())
            {
                if (context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.StructDeclaration != null)
                    context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.CloseStruct();

                context.CloseContext();
            }
        }
    }
}