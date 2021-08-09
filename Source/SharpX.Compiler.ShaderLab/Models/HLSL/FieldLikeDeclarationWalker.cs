using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Compiler.ShaderLab.Models.HLSL
{
    internal static class FieldLikeDeclarationWalker
    {
        public static bool ShouldProcessFieldLikeDeclarations(ILanguageSyntaxActionContext context)
        {
            return context.IsContextOpened();
        }

        public static void ProcessPropertyDeclarations(ILanguageSyntaxActionContext context)
        {
            if (context.Node is not PropertyDeclarationSyntax node)
                return;

            if (node.Initializer != null)
                context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab does not support property initializer"));

            if (node.AccessorList != null)
                foreach (var accessor in node.AccessorList.Accessors)
                    if (accessor.Body != null || accessor.ExpressionBody != null)
                        context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab does not support property expression bodies in set/get accessors"));

            if (node.ExpressionBody != null)
                context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab does not support property expression bodies"));

            ProcessFieldLikeDeclarations(context);
        }

        public static void ProcessFieldDeclarations(ILanguageSyntaxActionContext context)
        {
            if (context.Node is not FieldDeclarationSyntax node)
                return;

            if (node.Declaration.Variables.Count > 1)
                context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab does not support multiple declarations on single field declaration"));
            if (node.Declaration.Variables.Any(w => w.Identifier != default))
                context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab does not support field initializer"));

            ProcessFieldLikeDeclarations(context);
        }

        private static void ProcessFieldLikeDeclarations(ILanguageSyntaxActionContext context)
        {
            var capture = FieldLikeDeclarationCapture.Capture(context.Node, context.SemanticModel);
            if (capture == null)
            {
                context.Errors.Add(new DefaultError(context.Node, "Failed to load field-like syntax structure"));
                context.StopPropagation();
                return;
            }

            if (capture.HasAttribute<ExternalAttribute>())
                return;

            if (capture.HasAttribute<GlobalMemberAttribute>())
            {
                if (!capture.IsStaticMember())
                    context.Warnings.Add(new DefaultError(context.Node, "SharpX.ShaderLab recommended that global members be defined as static properties or fields"));

                var t = capture.GetDeclaredType();
                context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.AddGlobalMember(t.GetActualName(), capture.GetIdentifierName());
                return;
            }

            if (capture.HasAttribute<SemanticAttribute>())
            {
                if (capture.IsStaticMember())
                    context.Warnings.Add(new DefaultError(context.Node, "SharpX.ShaderLab recommend that semantic members be defined as instance properties or fields"));

                var attr = capture.GetAttribute<SemanticAttribute>()!;
                var t = capture.GetDeclaredType();
                context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.AddMemberToStruct(t.GetActualName(), capture.GetIdentifierName(), attr.Semantic);
            }
            else
            {
                if (capture.IsStaticMember())
                    context.Warnings.Add(new DefaultError(context.Node, "SharpX.ShaderLab recommend that semantic members be defined as instance properties or fields"));

                var t = capture.GetDeclaredType();
                context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.AddMemberToStruct(t.GetActualName(), capture.GetIdentifierName(), null);
            }
        }
    }
}