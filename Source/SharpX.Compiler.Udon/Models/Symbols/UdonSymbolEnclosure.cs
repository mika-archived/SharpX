using System;

namespace SharpX.Compiler.Udon.Models.Symbols
{
    internal class UdonSymbolEnclosure : IDisposable
    {
        private readonly UdonCSharpSyntaxWalker _walker;

        public UdonSymbolEnclosure(UdonCSharpSyntaxWalker walker)
        {
            _walker = walker;
            _walker.SymbolTable?.OpenSymbolTable();
        }

        public void Dispose()
        {
            _walker.SymbolTable?.CloseSymbolTable();
        }
    }
}