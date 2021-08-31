namespace SharpX.Compiler.Udon.Models.Symbols
{
    internal interface IAddressableSymbol : ISymbol
    {
        string Address { get; }

        string ToAddressString();
    }
}