using System.Collections.Generic;

namespace SharpX.Compiler.Udon.Models.UdonAssembly
{
    internal class OnlyComment : IAssemblyOpCode
    {
        public uint IncrementalProgramCounter => 0;

        public uint ActualProgramCounter { get; set; }
        
        public string? Comment { get; init; }

        public string ToAssemblyString(List<IAssemblyOpCode> inheritCodes)
        {
            if (string.IsNullOrWhiteSpace(Comment))
                return "";
            return $"# {Comment}";
        }
    }
}