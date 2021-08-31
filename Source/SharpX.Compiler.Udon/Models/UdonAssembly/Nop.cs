using System.Collections.Generic;

namespace SharpX.Compiler.Udon.Models.UdonAssembly
{
    internal class Nop : IAssemblyOpCode
    {
        public uint IncrementalProgramCounter => 4;

        public uint ActualProgramCounter { get; set; }

        public string? Comment { get; init; }

        public string ToAssemblyString(List<IAssemblyOpCode> inheritCodes)
        {
            if (string.IsNullOrWhiteSpace(Comment))
                return "NOP";
            return $"NOP # {Comment}";
        }
    }
}