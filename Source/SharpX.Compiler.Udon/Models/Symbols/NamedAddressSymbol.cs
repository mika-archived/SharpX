namespace SharpX.Compiler.Udon.Models.Symbols
{
    internal record NamedAddressSymbol(string Name, long RawAddress) : IAddressSymbol
    {
        public string Address => $"0x{RawAddress:X8}";
    }
}