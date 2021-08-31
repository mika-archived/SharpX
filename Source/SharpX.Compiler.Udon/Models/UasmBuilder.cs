using System.Collections.Generic;
using System.Linq;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Extensions;
using SharpX.Compiler.Udon.Enums;
using SharpX.Compiler.Udon.Models.Symbols;
using SharpX.Library.Udon.Enums;

namespace SharpX.Compiler.Udon.Models
{
    internal class UasmBuilder
    {
        private readonly Stack<MethodSymbol> _currentMethodSymbol;
        private readonly List<MethodSymbol> _methods;
        private readonly SourceBuilder _sb;
        private readonly List<UdonSymbol> _variables;

        public MethodUasmBuilder? CurrentMethodAssemblyBuilder => _currentMethodSymbol.Count > 0 ? _currentMethodSymbol.Peek().UAssembly : null;

        public uint CurrentProgramCounter { get; }

        public UasmBuilder()
        {
            _sb = new SourceBuilder();
            _variables = new List<UdonSymbol>();
            _methods = new List<MethodSymbol>();
            _currentMethodSymbol = new Stack<MethodSymbol>();
            CurrentProgramCounter = 0;
        }

        public UdonSymbol AddVariableSymbol(string name, string type, bool export, UdonSyncMode? sync, object? initialValue)
        {
            var symbol = new UdonSymbol(type, name, name, UdonSymbolDeclarations.Public, initialValue, null, export, sync);
            _variables.Add(symbol);

            return symbol;
        }

        public void AppendLine(string str, string? comment = null)
        {
            _sb.WriteLineWithIndent(string.IsNullOrWhiteSpace(comment) ? str : $"{str} # {comment}");
        }

        public void AppendCommentLine(string comment)
        {
            _sb.WriteLineWithIndent($"# {comment}");
        }

        public void StartMethod(string name, string @return, string[] arguments, bool export)
        {
            _currentMethodSymbol.Push(new MethodSymbol(name, @return, arguments, export, _methods.Count == 0 ? 0 : _methods.Last().UAssembly.CalcMethodLastAddress()));
        }

        public void CloseMethod()
        {
            var method = _currentMethodSymbol.Pop();
            _methods.Add(method);
        }

        public IReadOnlySet<string> GetHeapExternSignatures()
        {
            var set = new HashSet<string>();
            _methods.SelectMany(w => w.UAssembly.ExternSignatures).ForEach(w => set.Add(w));

            return set;
        }

        public string ToAssemblyString()
        {
            if (_variables.Count > 0)
            {
                _sb.WriteLineWithIndent(".data_start");
                _sb.IncrementIndent();
                _sb.WriteNewLine();

                if (_variables.Any(w => w.IsExport))
                {
                    foreach (var symbol in _variables.Where(w => w.IsExport))
                        _sb.WriteLineWithIndent($".export {symbol.UniqueName}");

                    _sb.WriteNewLine();
                }

                if (_variables.Any(w => w.SyncMode != null))
                {
                    foreach (var symbol in _variables.Where(w => w.SyncMode != null))
                        _sb.WriteLineWithIndent($".sync {symbol.UniqueName}, {symbol.SyncMode!.ToString()!.ToLowerInvariant()}");

                    _sb.WriteNewLine();
                }

                foreach (var symbol in _variables)
                    _sb.WriteLineWithIndent($"{symbol.UniqueName}: %{symbol.Type}, {symbol.ConstantValue ?? "null"}");

                _sb.WriteNewLine();
                _sb.DecrementIndent();
                _sb.WriteLineWithIndent(".data_end");
            }

            if (_methods.Count > 0)
            {
                _sb.WriteLineWithIndent(".code_start");
                _sb.IncrementIndent();
                _sb.WriteNewLine();

                foreach (var symbol in _methods)
                {
                    if (symbol.IsExport)
                    {
                        _sb.WriteLineWithIndent($".export {symbol.Name}");
                        _sb.WriteNewLine();
                    }

                    _sb.WriteLineWithIndent($"{symbol.Name}:");
                    _sb.IncrementIndent();

                    symbol.UAssembly.WriteTo(_sb);

                    _sb.WriteNewLine();
                    _sb.DecrementIndent();
                }

                _sb.WriteNewLine();
                _sb.DecrementIndent();
                _sb.WriteLineWithIndent(".code_end");
            }

            return _sb.ToSource();
        }
    }
}