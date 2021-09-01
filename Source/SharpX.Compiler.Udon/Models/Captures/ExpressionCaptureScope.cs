using System;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Udon.Models.Symbols;

namespace SharpX.Compiler.Udon.Models.Captures
{
    internal class ExpressionCaptureScope : IDisposable
    {
        private readonly UdonCSharpSyntaxWalker _walker;

        public SafeStack<UdonSymbol> CapturingExpressions { get; }

        public UdonSymbol? DestinationSymbol { get; }

        public ExpressionCaptureScope(UdonCSharpSyntaxWalker walker, bool isGetterContext = false, UdonSymbol? destination = null)
        {
            _walker = walker;

             CapturingExpressions = new SafeStack<UdonSymbol>();
            DestinationSymbol = destination;

            _walker.ExpressionCapturingStack.Push(CapturingExpressions);
            _walker.IsGetterContext.Push(isGetterContext);
            _walker.DestinationSymbolStack.Push(destination);
        }

        public void Dispose()
        {
            _walker.DestinationSymbolStack.Pop();
            _walker.IsGetterContext.Pop();
            _walker.ExpressionCapturingStack.Pop();
        }
    }
}