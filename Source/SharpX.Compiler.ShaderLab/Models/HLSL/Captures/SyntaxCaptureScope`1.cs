using System;

using SharpX.Compiler.Composition.Enums;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Captures
{
    public class SyntaxCaptureScope<T> : IDisposable where T : INestableStatement
    {
        private readonly WellKnownSyntax _syntax;
        private readonly ShaderLabCSharpSyntaxWalker _walker;

        public T Statement { get; }

        private SyntaxCaptureScope(ShaderLabCSharpSyntaxWalker walker, WellKnownSyntax syntax, T statement)
        {
            _walker = walker;
            _syntax = syntax;
            Statement = statement;
        }

        public void Dispose()
        {
            _walker.CapturingStack.Pop();
            _walker.StatementStack.Pop();
        }

        public static SyntaxCaptureScope<T> Create(ShaderLabCSharpSyntaxWalker walker, WellKnownSyntax syntax, T statement)
        {
            var scope = new SyntaxCaptureScope<T>(walker, syntax, statement);
            scope._walker.CapturingStack.Push(syntax);
            scope._walker.StatementStack.Push(statement);

            return scope;
        }
    }
}