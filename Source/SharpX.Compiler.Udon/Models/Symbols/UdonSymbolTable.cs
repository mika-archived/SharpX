using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpX.Compiler.Udon.Models.Symbols
{
    internal class UdonSymbolTable : IDisposable
    {
        private readonly Dictionary<string, uint> _nameCounter;
        private readonly List<NamedAddressSymbol> _namedAddressSymbols;
        private readonly List<VariableSymbol> _namedSymbols;

        public IReadOnlyCollection<NamedAddressSymbol> NamedAddressSymbols => _namedAddressSymbols.AsReadOnly();

        public IReadOnlyCollection<VariableSymbol> VariableSymbols => _namedSymbols.AsReadOnly();

        public UdonSymbolTable()
        {
            _namedAddressSymbols = new List<NamedAddressSymbol>();
            _namedSymbols = new List<VariableSymbol>();
            _nameCounter = new Dictionary<string, uint>();
        }

        public void Dispose() { }

        public NamedAddressSymbol GetConstantSymbol(string name, long value)
        {
            if (_namedAddressSymbols.Any(w => w.RawAddress == value))
                return _namedAddressSymbols.First(w => w.RawAddress == value);

            SetNamedCounter(name, out var counter);

            var symbol = new NamedAddressSymbol($"__internal_const_{name}_SystemUInt32_{counter}", value);
            _namedAddressSymbols.Add(symbol);

            return symbol;
        }

        public VariableSymbol CreateNamedSymbol(string name, string type, bool isThis = false)
        {
            SetNamedCounter(name, out var counter);

            var symbol = new VariableSymbol($"__internal_variable_{name}_{type}_{counter}", type, false, null, isThis ? "this" : "null");
            _namedSymbols.Add(symbol);

            return symbol;
        }

        public VariableSymbol? GetNamedSymbol(string name)
        {
            return _namedSymbols.FirstOrDefault(w => w.Name == name);
        }

        public void AddNamedSymbol(VariableSymbol symbol)
        {
            if (_namedSymbols.Contains(symbol))
                return;

            _namedSymbols.Add(symbol);
            SetNamedCounter(symbol.Name, out _);
        }

        public void SetNamedCounter(string name, out uint counter)
        {
            if (_nameCounter.ContainsKey(name))
            {
                _nameCounter[name]++;
                counter = _nameCounter[name];
            }
            else
            {
                _nameCounter.Add(name, 0);
                counter = 0;
            }
        }
    }
}