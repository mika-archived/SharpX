using System.Collections.Generic;

namespace SharpX.Compiler.Udon.Models.UdonAssembly
{
    internal class Extern : IAssemblyOpCode
    {
        public string Signature { get; init; }

        public uint IncrementalProgramCounter => 8;

        public uint ActualProgramCounter { get; set; }

        public string? Comment { get; init; }

        public string ToAssemblyString(List<IAssemblyOpCode> inheritCodes)
        {
            if (string.IsNullOrWhiteSpace(Comment))
                return $"EXTERN, \"{Signature}\"";
            return $"EXTERN, \"{Signature}\" # {Comment}";
        }
    }
}