using SharpX.Library.Udon.Enums;

namespace SharpX.Compiler.Udon.Models.Symbols
{
    public record VariableSymbol(string Name, string Type, bool IsExport, UdonSyncMode? SyncMode, object? InitialValue) : IAddressableSymbol
    {
        public string Address { get; }

        public string ToAddressString()
        {
            return Name;
        }
    }
}