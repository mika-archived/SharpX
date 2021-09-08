using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.Composition.Abstractions.Errors
{
    public class VisualStudioCatchError : IError
    {
        private readonly CapturedType _captured;
        private readonly Diagnostic? _diagnostic;
        private readonly string? _msg;
        private readonly CSharpSyntaxNode? _node;
        private readonly int? _code;

        public VisualStudioCatchError(CSharpSyntaxNode node, string message, int? code = null)
        {
            _node = node;
            _msg = message;
            _code = code;
            _captured = CapturedType.Node;
        }

        public VisualStudioCatchError(Diagnostic diagnostic)
        {
            _diagnostic = diagnostic;
            _captured = CapturedType.Diagnostic;
        }

        public string GetMessage()
        {
            return _captured switch
            {
                CapturedType.Diagnostic => GetMessage(_diagnostic!),
                CapturedType.Node => GetMessage(_node!, _msg!, _code),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static string GetMessage(Diagnostic diagnostic)
        {
            var path = diagnostic.Location.SourceTree?.FilePath ?? "InMemory.cs";
            var position = diagnostic.Location.GetLineSpan().StartLinePosition;
            return $"{path}({position.Line + 1},{position.Character}): Error {diagnostic.Id}: {diagnostic.GetMessage()}";
        }

        private static string GetMessage(CSharpSyntaxNode node, string msg, int? code, string category = "SXC")
        {
            var path = node.SyntaxTree.FilePath;
            var position = node.GetLocation().GetLineSpan().StartLinePosition;
            return $"{path}({position.Line + 1},{position.Character}): Error {category}{(code.HasValue ? code.Value.ToString("0000") : "9999")}: {msg}";
        }

        private enum CapturedType
        {
            Node,

            Diagnostic
        }
    }
}