namespace SharpX.Compiler.Udon.Models.Symbols
{
    internal record AddressSymbol(long RawAddress)
    {
        public string Address => $"0x{RawAddress:X8}";
    }
}