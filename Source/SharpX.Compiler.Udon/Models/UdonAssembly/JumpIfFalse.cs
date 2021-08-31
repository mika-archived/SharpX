using System.Collections.Generic;

using SharpX.Compiler.Udon.Models.Symbols;

namespace SharpX.Compiler.Udon.Models.UdonAssembly
{
    internal class JumpIfFalse : IAssemblyOpCode
    {
        public IAddressableSymbol Address { get; init; }

        public uint IncrementalProgramCounter => 8;

        public uint ActualProgramCounter { get; set; }

        public string? Comment { get; init; }

        public string ToAssemblyString(List<IAssemblyOpCode> inheritCodes)
        {
            if (string.IsNullOrWhiteSpace(Comment))
                return $"JUMP_IF_FALSE, {Address.ToAddressString()}";
            return $"JUMP_IF_FALSE, {Address.ToAddressString()} # {Comment}";
        }
    }
}