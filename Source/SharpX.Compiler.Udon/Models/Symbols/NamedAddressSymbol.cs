namespace SharpX.Compiler.Udon.Models.Symbols
{
    internal record NamedAddressSymbol(string Name, long RawAddress) : IAddressableSymbol
    {
        public string Address => $"0x{RawAddress:X8}";

        public string ToAddressString()
        {
            return Name;
        }
    }
}