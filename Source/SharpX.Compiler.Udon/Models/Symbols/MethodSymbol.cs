﻿namespace SharpX.Compiler.Udon.Models.Symbols
{
    internal record MethodSymbol(string Name, string ReturnType, string[] Arguments, bool IsExport)
    {
        public UasmBuilder UAssembly { get; init; } = new();
    }
}