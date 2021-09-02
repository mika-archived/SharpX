using System.Collections.Generic;
using System.Linq;

namespace SharpX.Compiler.Udon.Models.UdonAssembly
{
    internal class JumpIfFalseLabel : IAssemblyOpCode
    {
        public string Label { get; init; }

        public uint IncrementalProgramCounter => 8;

        public uint ActualProgramCounter { get; set; }

        public string? Comment { get; init; }

        public string ToAssemblyString(IReadOnlyList<IAssemblyOpCode> inheritCodes)
        {
            var addr = inheritCodes.OfType<Label>().First(w => w.Name == Label);
            if (string.IsNullOrWhiteSpace(Comment))
                return $"JUMP_IF_FALSE, 0x{addr.ActualProgramCounter:X8}";
            return $"JUMP_IF_FALSE, 0x{addr.ActualProgramCounter:X8} # {Comment}";
        }
    }
}