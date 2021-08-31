using SharpX.Compiler.Udon.Enums;
using SharpX.Library.Udon.Enums;

namespace SharpX.Compiler.Udon.Models.Symbols
{
    internal record UdonSymbol(string Type, string UniqueName, string? OriginalName, UdonSymbolDeclarations Declaration, object? HeapInitialValue, object? ConstantValue, bool IsExport = false, UdonSyncMode? SyncMode = null)
    {
        public string ToAddressString()
        {
            return UniqueName;
        }
    }
}