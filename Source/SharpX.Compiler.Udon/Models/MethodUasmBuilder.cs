using System.Collections.Generic;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Udon.Models.Symbols;

namespace SharpX.Compiler.Udon.Models
{
    internal class MethodUasmBuilder
    {
        private readonly List<string> _codes;

        public MethodUasmBuilder()
        {
            _codes = new List<string>();
        }

        public void AddComment(string comment)
        {
            _codes.Add($"# {comment}");
        }

        public void AddRaw(string raw, string? comment = null)
        {
            _codes.Add(string.IsNullOrWhiteSpace(comment) ? raw : $"{raw} # {comment}");
        }
        public void AddNop(string? comment = null)
        {
            _codes.Add(string.IsNullOrWhiteSpace(comment) ? "NOP" : $"NOP # {comment}");
        }

        public void AddPush(VariableSymbol symbol, string? comment = null)
        {
            _codes.Add(string.IsNullOrWhiteSpace(comment) ? $"PUSH, {symbol.Name}" : $"PUSH, {symbol.Name} # {comment}");
        }

        public void AddPush(AddressSymbol address, string? comment = null)
        {
            _codes.Add(string.IsNullOrWhiteSpace(comment) ? $"PUSH, {address.Address}" : $"PUSH, {address.Address} # {comment}");
        }

        public void AddPop(string? comment = null)
        {
            _codes.Add(string.IsNullOrWhiteSpace(comment) ? "POP" : $"POP # {comment}");
        }
        
        public void AddJumpIfFalse(AddressSymbol address, string? comment = null)
        {
            _codes.Add(string.IsNullOrWhiteSpace(comment) ? $"JUMP_IF_FALSE {address.Address}" : $"JUMP_IF_FALSE {address.Address} # {comment}");
        }

        public void AddJump(AddressSymbol address, string? comment = null)
        {
            _codes.Add(string.IsNullOrWhiteSpace(comment) ? $"JUMP {address.Address}" : $"JUMP {address.Address} # {comment}");
        }

        public void AddExtern(string signature, string? comment = null)
        {
            _codes.Add(string.IsNullOrWhiteSpace(comment) ? $"EXTERN, \"{signature}\"" : $"EXTERN \"{signature}\" # {comment}");
        }

        public void AddJumpIndirect(AddressSymbol address, string? comment = null)
        {
            _codes.Add(string.IsNullOrWhiteSpace(comment) ? $"JUMP_INDIRECT {address.Address}" : $"JUMP_INDIRECT {address.Address} # {comment}");
        }

        public void AddCopy(string? comment = null)
        {
            _codes.Add(string.IsNullOrWhiteSpace(comment) ? "COPY" : $"COPY # {comment}");
        }


        public void WriteTo(SourceBuilder sb)
        {
            foreach (var code in _codes)
                sb.WriteLineWithIndent(code);
        }
    }
}