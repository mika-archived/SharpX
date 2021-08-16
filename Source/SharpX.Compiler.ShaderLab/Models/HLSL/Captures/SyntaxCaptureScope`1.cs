using System;
using System.Collections.Generic;

using SharpX.Compiler.Composition.Enums;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Captures
{
    public class SyntaxCaptureScope<T> : IDisposable where T : INestableStatement
    {
        private readonly WellKnownSyntax _syntax;
        private readonly ShaderLabCSharpSyntaxWalker _walker;

        public T Statement { get; }

        public Dictionary<string, object> Metadata { get; }

        private SyntaxCaptureScope(ShaderLabCSharpSyntaxWalker walker, WellKnownSyntax syntax, T statement, Dictionary<string, object>? metadata)
        {
            _walker = walker;
            _syntax = syntax;
            Statement = statement;
            Metadata = metadata ?? new Dictionary<string, object>();
        }

        public void Dispose()
        {
            _walker.CapturingStack.Pop();
            _walker.StatementStack.Pop();
            _walker.MetadataStack.Pop();
        }

        public static SyntaxCaptureScope<T> Create(ShaderLabCSharpSyntaxWalker walker, WellKnownSyntax syntax, T statement, bool isInherit = false)
        {
            var scope = new SyntaxCaptureScope<T>(walker, syntax, statement, isInherit ? walker.Metadata : null);
            scope._walker.CapturingStack.Push(syntax);
            scope._walker.StatementStack.Push(statement);
            scope._walker.MetadataStack.Push(scope.Metadata);

            return scope;
        }
    }
}