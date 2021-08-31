using System;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Udon.Models.Symbols;

namespace SharpX.Compiler.Udon.Models.Captures
{
    internal class MethodCapturingScope : IDisposable
    {
        private readonly UdonSymbolEnclosure _enclosure;
        private readonly UdonCSharpSyntaxWalker _walker;

        public MethodCapturingScope(UdonCSharpSyntaxWalker walker, MethodDeclarationSyntax node)
        {
            _walker = walker;
            _walker.MethodCapturingStack.Push(UdonNodeResolver.Instance.RemappedBuiltinEvent(node.Identifier.ValueText));
            _enclosure = new UdonSymbolEnclosure(walker);
        }


        public void Dispose()
        {
            _walker.MethodCapturingStack.Pop();
            _enclosure.Dispose();
        }
    }
}