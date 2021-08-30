using System.Collections.Generic;
using System.Linq;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Udon.Models.Symbols;
using SharpX.Library.Udon.Enums;

namespace SharpX.Compiler.Udon.Models
{
    internal class UasmBuilder
    {
        private readonly Stack<MethodSymbol> _currentMethodSymbol;
        private readonly List<MethodSymbol> _methods;
        private readonly SourceBuilder _sb;
        private readonly List<VariableSymbol> _variables;

        public MethodUasmBuilder? CurrentMethodAssemblyBuilder => _currentMethodSymbol.Count > 0 ? _currentMethodSymbol.Peek().UAssembly : null;

        public UasmBuilder()
        {
            _sb = new SourceBuilder();
            _variables = new List<VariableSymbol>();
            _methods = new List<MethodSymbol>();
            _currentMethodSymbol = new Stack<MethodSymbol>();
        }

        public void AddVariableSymbol(string name, string type, bool export, UdonSyncMode? sync, object? initialValue)
        {
            _variables.Add(new VariableSymbol(name, type, export, sync, initialValue));
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
            _currentMethodSymbol.Push(new MethodSymbol(name, @return, arguments, export));
        }

        public void CloseMethod()
        {
            var method = _currentMethodSymbol.Pop();
            _methods.Add(method);
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
                        _sb.WriteLineWithIndent($".export {symbol.Name}");

                    _sb.WriteNewLine();
                }

                if (_variables.Any(w => w.SyncMode != null))
                {
                    foreach (var symbol in _variables.Where(w => w.SyncMode != null))
                        _sb.WriteLineWithIndent($".sync {symbol.Name}, {symbol.SyncMode!.ToString()!.ToLowerInvariant()}");

                    _sb.WriteNewLine();
                }

                foreach (var symbol in _variables)
                    _sb.WriteLineWithIndent($"{symbol.Name}: %{symbol.Type}, {symbol.InitialValue ?? "null"}");

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