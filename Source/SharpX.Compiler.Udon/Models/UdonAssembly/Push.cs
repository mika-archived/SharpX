using System.Collections.Generic;

using SharpX.Compiler.Udon.Models.Symbols;

namespace SharpX.Compiler.Udon.Models.UdonAssembly
{
    internal class Push : IAssemblyOpCode
    {
        public UdonSymbol Address { get; init; }

        public uint IncrementalProgramCounter => 8;

        public uint ActualProgramCounter { get; set; }

        public string? Comment { get; init; }

        public string ToAssemblyString(List<IAssemblyOpCode> inheritCodes)
        {
            if (string.IsNullOrWhiteSpace(Comment))
                return $"PUSH, {Address.ToAddressString()}";
            return $"PUSH, {Address.ToAddressString()} # {Comment}";
        }
    }
}