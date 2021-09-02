﻿using System.Collections.Generic;
using System.Linq;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Udon.Models.Symbols;
using SharpX.Compiler.Udon.Models.UdonAssembly;

namespace SharpX.Compiler.Udon.Models
{
    internal class MethodUasmBuilder
    {
        private readonly List<IAssemblyOpCode> _codes;
        private readonly uint _programCounterOffset;
        private readonly HashSet<string> _externSignatures;

        private uint CurrentProgramCounter => _programCounterOffset + (uint)_codes.Sum(w => w.IncrementalProgramCounter);

        public IReadOnlySet<string> ExternSignatures => _externSignatures;

        public MethodUasmBuilder(uint programCounterOffset)
        {
            _programCounterOffset = programCounterOffset;
            _codes = new List<IAssemblyOpCode>();
            _externSignatures = new HashSet<string>();
        }

        public void AddBlankLine()
        {
            _codes.Add(new OnlyComment());
        }

        public void AddComment(string comment)
        {
            _codes.Add(new OnlyComment { Comment = comment });
        }

        public void AddNop(string? comment = null)
        {
            var code = new Nop
            {
                Comment = comment,
                ActualProgramCounter = CurrentProgramCounter
            };

            _codes.Add(code);
        }

        public void AddLabel(string label, string? comment = null)
        {
            var code = new Label
            {
                Name = label,
                Comment = comment,
                ActualProgramCounter = CurrentProgramCounter,
            };

            _codes.Add(code);
        }

        public void AddPush(UdonSymbol symbol, string? comment = null)
        {
            var code = new Push
            {
                Address = symbol,
                Comment = comment,
                ActualProgramCounter = CurrentProgramCounter
            };

            _codes.Add(code);
        }

        public void AddPushBeforeCurrent(UdonSymbol symbol, int index = 1, string? comment = null)
        {
            var code = new Push
            {
                Address = symbol,
                Comment = comment,
                ActualProgramCounter = CurrentProgramCounter,
            };

            _codes.Insert(_codes.Count - index, code);
            RecalculateProgramCounter();
        }

        public void AddPop(string? comment = null)
        {
            var code = new Pop
            {
                Comment = comment,
                ActualProgramCounter = CurrentProgramCounter
            };

            _codes.Add(code);
        }

        public void AddJumpIfFalse(UdonSymbol address, string? comment = null)
        {
            var code = new JumpIfFalse
            {
                Address = address,
                Comment = comment,
                ActualProgramCounter = CurrentProgramCounter
            };

            _codes.Add(code);
        }

        public void AddJumpIfFalseLabel(string label, string? comment = null)
        {
            var code = new JumpIfFalseLabel
            {
                Label = label,
                Comment = comment,
                ActualProgramCounter = CurrentProgramCounter
            };

            _codes.Add(code);
        }

        public void AddJump(UdonSymbol address, string? comment = null)
        {
            var code = new Jump
            {
                Address = address,
                Comment = comment,
                ActualProgramCounter = CurrentProgramCounter
            };

            _codes.Add(code);
        }

        public void AddJumpLabel(string label, string? comment = null)
        {
            var code = new JumpLabel
            {
                Label = label,
                Comment = comment,
                ActualProgramCounter = CurrentProgramCounter
            };

            _codes.Add(code);
        }

        public void AddExtern(string signature, string? comment = null)
        {
            _externSignatures.Add(signature);

            var code = new Extern
            {
                Signature = signature,
                Comment = comment,
                ActualProgramCounter = CurrentProgramCounter
            };

            _codes.Add(code);
        }

        public void AddJumpIndirect(UdonSymbol address, string? comment = null)
        {
            var code = new JumpIndirect
            {
                Symbol = address,
                Comment = comment,
                ActualProgramCounter = CurrentProgramCounter
            };

            _codes.Add(code);
        }

        public void AddCopy(string? comment = null)
        {
            var code = new Copy
            {
                Comment = comment,
                ActualProgramCounter = CurrentProgramCounter
            };

            _codes.Add(code);
        }

        public uint CalcMethodLastAddress()
        {
            var last = _codes.Last();
            return last.ActualProgramCounter + last.IncrementalProgramCounter;
        }

        public void WriteTo(SourceBuilder sb)
        {
            foreach (var code in _codes.AsReadOnly())
                sb.WriteLineWithIndent(code.ToAssemblyString(_codes));
        }

        private void RecalculateProgramCounter()
        {
            var counter = _programCounterOffset;

            foreach (var code in _codes)
            {
                code.ActualProgramCounter = counter;
                counter += code.IncrementalProgramCounter;
            }
        }
    }
}