using SharpX.Library.Udon.Enums;

namespace SharpX.Compiler.Udon.Models.Symbols
{
    internal record ConstantSymbol(string Name, string Type, bool IsExport, UdonSyncMode? SyncMode, object? InitialValue, object? ConstantValue) : VariableSymbol(Name, Type, IsExport, SyncMode, InitialValue) { }
}