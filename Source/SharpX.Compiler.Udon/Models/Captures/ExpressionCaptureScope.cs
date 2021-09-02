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

        public bool IsGetterContext { get; }

        public ExpressionCaptureScope(UdonCSharpSyntaxWalker walker, bool isGetterContext, UdonSymbol? destination = null)
        {
            _walker = walker;

             CapturingExpressions = new SafeStack<UdonSymbol>();
            DestinationSymbol = destination;
            IsGetterContext = isGetterContext;

            _walker.ExpressionCapturingStack.Push(CapturingExpressions);
            _walker.IsGetterContextStack.Push(isGetterContext);
            _walker.DestinationSymbolStack.Push(destination);
        }

        public void Dispose()
        {
            _walker.DestinationSymbolStack.Pop();
            _walker.IsGetterContextStack.Pop();
            _walker.ExpressionCapturingStack.Pop();
        }
    }
}