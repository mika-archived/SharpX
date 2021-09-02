using System.Collections.Generic;

using SharpX.Compiler.Udon.Models.Symbols;

namespace SharpX.Compiler.Udon.Models.UdonAssembly
{
    internal class JumpIndirect : IAssemblyOpCode
    {
        public UdonSymbol Symbol { get; init; }

        public uint IncrementalProgramCounter => 8;

        public uint ActualProgramCounter { get; set; }

        public string? Comment { get; init; }

        public string ToAssemblyString(IReadOnlyList<IAssemblyOpCode> inheritCodes)
        {
            if (string.IsNullOrWhiteSpace(Comment))
                return $"JUMP_INDIRECT, {Symbol.UniqueName}";
            return $"JUMP_INDIRECT, {Symbol.UniqueName} # {Comment}";
        }
    }
}