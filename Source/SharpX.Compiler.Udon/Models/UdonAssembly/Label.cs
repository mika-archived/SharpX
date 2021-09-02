using System.Collections.Generic;

namespace SharpX.Compiler.Udon.Models.UdonAssembly
{
    // internal used, not exists in Udon
    internal class Label : IAssemblyOpCode
    {
        public string Name { get; init; }

        public uint IncrementalProgramCounter => 0;

        public uint ActualProgramCounter { get; set; }

        public string? Comment { get; init; }

        public string ToAssemblyString(IReadOnlyList<IAssemblyOpCode> inheritCodes)
        {
            if (string.IsNullOrWhiteSpace(Comment))
                return $"# LABELED {Name}";
            return $"# LABELED {Name} - {Comment}";
        }
    }
}