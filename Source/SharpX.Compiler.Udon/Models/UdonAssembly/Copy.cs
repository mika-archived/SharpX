using System.Collections.Generic;

namespace SharpX.Compiler.Udon.Models.UdonAssembly
{
    internal class Copy : IAssemblyOpCode
    {
        public uint IncrementalProgramCounter => 4;

        public uint ActualProgramCounter { get; set; }

        public string? Comment { get; init; }

        public string ToAssemblyString(IReadOnlyList<IAssemblyOpCode> inheritCodes)
        {
            if (string.IsNullOrWhiteSpace(Comment))
                return "COPY";
            return $"COPY # {Comment}";
        }
    }
}