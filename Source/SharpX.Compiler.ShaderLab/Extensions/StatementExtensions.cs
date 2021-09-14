using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.ShaderLab.Models.HLSL.Statements;

namespace SharpX.Compiler.ShaderLab.Extensions
{
    internal static class StatementExtensions
    {
        public static Statement IntoStatement(this IStatement statement)
        {
            return new(SyntaxFactory.Token(SyntaxKind.SemicolonToken), statement);
        }
    }
}