using System.Collections.Generic;

namespace SharpX.Compiler.Udon.Models.UdonAssembly
{
    internal interface IAssemblyOpCode
    {
        uint IncrementalProgramCounter { get; }

        uint ActualProgramCounter { get; set; }

        string? Comment { get; init; }

        string ToAssemblyString(List<IAssemblyOpCode> inheritCodes);
    }
}