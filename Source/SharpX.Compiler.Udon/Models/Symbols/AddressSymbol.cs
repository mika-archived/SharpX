﻿namespace SharpX.Compiler.Udon.Models.Symbols
{
    internal record AddressSymbol(long RawAddress) : IAddressSymbol
    {
        public string Address => $"0x{RawAddress:X8}";
    }
}