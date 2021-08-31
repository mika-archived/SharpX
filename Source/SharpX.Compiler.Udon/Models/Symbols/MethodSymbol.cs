namespace SharpX.Compiler.Udon.Models.Symbols
{
    internal record MethodSymbol(string Name, string ReturnType, string[] Arguments, bool IsExport, uint StartAddress)
    {
        public MethodUasmBuilder UAssembly { get; init; } = new(StartAddress);
    }
}