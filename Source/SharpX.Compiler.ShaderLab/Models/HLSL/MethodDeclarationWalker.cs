using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.Extensions;
using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Compiler.ShaderLab.Models.HLSL
{
    public static class MethodDeclarationWalker
    {
        public static void PreProcessMethodDeclarations(ILanguageSyntaxActionContext context)
        {
            if (context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.StructDeclaration != null)
            {
                context.Errors.Add(new DefaultError(context.Node, "SharpX.ShaderLab does not allow methods to be defined inside a class defined as a component."));
                return;
            }

            if (context.Node.HasAttribute<ExternalAttribute>(context.SemanticModel))
                return;

            var name = GetDeclaredName(context);
            var returns = GetReturnType(context);
            var arguments = GetArguments(context);

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(returns))
                return; //THEORETICALLY UNREACHABLE

            context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.OpenFunction(name, returns);
            foreach (var argument in arguments)
                context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.FunctionDeclaration!.AddArgument(argument.Type, argument.Name);
        }

        public static void PostProcessMethodDeclarations(ILanguageSyntaxActionContext context)
        {
            if (context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.FunctionDeclaration == null)
                return;

            context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.CloseFunction();
        }

        private static string? GetDeclaredName(ILanguageSyntaxActionContext context)
        {
            if (context.Node is not MethodDeclarationSyntax node)
                return null;

            if (node.HasAttribute<FunctionAttribute>(context.SemanticModel))
            {
                var attr = node.GetAttribute<FunctionAttribute>(context.SemanticModel);
                if (attr?.IsValidName() == true)
                    return attr.Alternative;
            }

            return node.Identifier.ValueText;
        }

        private static string? GetReturnType(ILanguageSyntaxActionContext context)
        {
            if (context.Node is not MethodDeclarationSyntax node)
                return null;

            var capture = TypeDeclarationCapture.Capture(node.ReturnType, context.SemanticModel);
            return capture.GetActualName();
        }

        private static IEnumerable<Argument> GetArguments(ILanguageSyntaxActionContext context)
        {
            if (context.Node is not MethodDeclarationSyntax node)
                return new List<Argument>();

            var arguments = node.ParameterList.Parameters;
            return arguments.Select(w =>
            {
                if (w.Type == null)
                {
                    context.Errors.Add(new DefaultError(w, "unknown type declaration is detected"));
                    context.StopPropagation();
                    return null;
                }

                var t = TypeDeclarationCapture.Capture(w.Type, context.SemanticModel);
                return new Argument(t.GetActualName(), w.Identifier.ValueText, null);
            }).Where(w => w != null).ToList()!;
        }

        private record Argument(string Type, string Name, string? Attribute);
    }
}