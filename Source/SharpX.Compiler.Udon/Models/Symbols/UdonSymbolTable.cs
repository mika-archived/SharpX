using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Udon.Enums;

namespace SharpX.Compiler.Udon.Models.Symbols
{
    internal class UdonSymbolTable
    {
        private readonly Dictionary<ISymbol, UdonSymbol> _associatedSymbols;
        private readonly List<UdonSymbolTable> _childSymbolTables;
        private readonly List<UdonSymbol> _contextDefinedSymbols;
        private readonly List<UdonSymbol> _contextReferenceableSymbols;
        private readonly Dictionary<string, uint> _counter;
        private readonly UdonSymbolTable _rootSymbolTable;
        private readonly SafeStack<UdonSymbolTable> _stack;

        private UdonSymbolTable CurrentContextSymbolTable => _stack.SafePeek() ?? this;

        public IReadOnlyCollection<UdonSymbol> ContextReferenceableSymbols => _stack.SafePeek(this)._contextReferenceableSymbols.AsReadOnly();

        public IReadOnlyCollection<UdonSymbol> ContextDefinedSymbols => _stack.SafePeek(this)._contextDefinedSymbols.AsReadOnly();

        [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1024:Compare symbols correctly", Justification = "<Pending>")]
        public UdonSymbolTable()
        {
            _associatedSymbols = new Dictionary<ISymbol, UdonSymbol>(new ISymbolEqualityComparer());
            _childSymbolTables = new List<UdonSymbolTable>();
            _contextDefinedSymbols = new List<UdonSymbol>();
            _contextReferenceableSymbols = new List<UdonSymbol>();
            _counter = new Dictionary<string, uint>();
            _rootSymbolTable = this;
            _stack = new SafeStack<UdonSymbolTable>();
        }

        [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1024:Compare symbols correctly", Justification = "<Pending>")]
        private UdonSymbolTable(UdonSymbolTable root, UdonSymbolTable parent)
        {
            _associatedSymbols = new Dictionary<ISymbol, UdonSymbol>(parent._associatedSymbols, new ISymbolEqualityComparer());
            _childSymbolTables = new List<UdonSymbolTable>(); // unused
            _contextDefinedSymbols = new List<UdonSymbol>();
            _contextReferenceableSymbols = new List<UdonSymbol>(parent._contextReferenceableSymbols);
            _counter = new Dictionary<string, uint>(); // unused
            _rootSymbolTable = root;
            _stack = new SafeStack<UdonSymbolTable>(); // unused
        }

        private UdonSymbolTable Inherit()
        {
            return new UdonSymbolTable(_rootSymbolTable, this);
        }

        public void OpenSymbolTable()
        {
            _stack.Push(Inherit());
        }

        public void CloseSymbolTable()
        {
            var child = _stack.Pop();
            _childSymbolTables.Add(child);

            foreach (var s in child._associatedSymbols.Keys.Where(s => !_associatedSymbols.ContainsKey(s)))
                _associatedSymbols.Add(s, child._associatedSymbols[s]);
        }

        public void ToFlatten()
        {
            foreach (var table in _childSymbolTables)
                _contextDefinedSymbols.AddRange(table._contextDefinedSymbols);
        }

        public UdonSymbol CreateOrGetNamedConstantSymbol(string type, string name, object? value)
        {
            if (TryGetGlobalSymbol(type, name, value, UdonSymbolDeclarations.PrivateConstant, out var symbol))
                return symbol;

            IncrementNameCounter(type, name, out var counter);

            var u = GetSymbolSignature(counter, type, name, UdonSymbolDeclarations.PrivateConstant);
            var s = new UdonSymbol(type, u, name, UdonSymbolDeclarations.PrivateConstant, value, "null");
            _rootSymbolTable.AddNewSymbol(s);

            return s;
        }

        public UdonSymbol CreateOrGetUnnamedConstantSymbol(string type, object? value)
        {
            if (TryGetGlobalSymbol(type, null, value, UdonSymbolDeclarations.PrivateConstant, out var symbol))
                return symbol;

            IncrementNameCounter(type, null, out var counter);

            var u = GetSymbolSignature(counter, type, null, UdonSymbolDeclarations.PrivateConstant);
            var s = new UdonSymbol(type, u, null, UdonSymbolDeclarations.PrivateConstant, value, "null");
            _rootSymbolTable.AddNewSymbol(s);

            return s;
        }

        public UdonSymbol CreateNamedThisSymbol(string type, string name)
        {
            IncrementNameCounter(type, name, out var counter);

            var u = GetSymbolSignature(counter, type, name, UdonSymbolDeclarations.This);
            var s = new UdonSymbol(type, u, name, UdonSymbolDeclarations.This, null, "this");
            AddNewSymbol(s);

            return s;
        }

        public UdonSymbol CreateUnnamedThisSymbol(string type, object? heapValue = null)
        {
            IncrementNameCounter(type, null, out var counter);

            var u = GetSymbolSignature(counter, type, null, UdonSymbolDeclarations.This);
            var s = new UdonSymbol(type, u, null, UdonSymbolDeclarations.This, heapValue, "this");
            AddNewSymbol(s);

            return s;
        }

        public UdonSymbol CreateOrGetNamedSymbol(string type, string name, UdonSymbolDeclarations declaration = UdonSymbolDeclarations.Local)
        {
            if (TryGetGlobalSymbol(type, name, "null", declaration, out var symbol))
                return symbol;
            return CreateNamedSymbol(type, name, declaration);
        }

        public UdonSymbol CreateNamedSymbol(string type, string name, UdonSymbolDeclarations declaration = UdonSymbolDeclarations.Local)
        {
            IncrementNameCounter(type, name, out var counter);

            var u = GetSymbolSignature(counter, type, name, declaration);
            var s = new UdonSymbol(type, u, name, declaration, "null", null);
            AddNewSymbol(s);

            return s;
        }

        public UdonSymbol CreateParameterNamedSymbol(IMethodSymbol m, string type, string name)
        {
            var signature = $"__{m.Name}__parameter__{name}_{type}";

            if (TryGetGlobalSymbol(type, signature, "null", UdonSymbolDeclarations.MethodParameter, out _))
                throw new InvalidOperationException();

            var symbol = new UdonSymbol(type, signature, signature, UdonSymbolDeclarations.MethodParameter, "null", null);
            AddNewSymbol(symbol);

            return symbol;
        }

        public UdonSymbol CreateReturnUnnamedSymbol(IMethodSymbol m, SemanticModel model, string type)
        {
            var parameters = string.Join("_", m.Parameters.Select(w => UdonNodeResolver.Instance.GetUdonTypeName(w.Type, model)));
            var signature = $"__{m.Name}__{parameters}__returnValue_{type}";

            if (TryGetGlobalSymbol(type, signature, "null", UdonSymbolDeclarations.Private, out _))
                throw new InvalidOperationException();

            var symbol = new UdonSymbol(type, signature, signature, UdonSymbolDeclarations.Private, "null", null);
            AddNewSymbol(symbol);

            return symbol;
        }

        public UdonSymbol? GetNamedSymbol(string type, string name, UdonSymbolDeclarations declaration = UdonSymbolDeclarations.Local)
        {
            if (TryGetSymbol(type, name, null, declaration, out var symbol))
                return symbol;
            return null;
        }

        public UdonSymbol CreateUnnamedSymbol(string type, UdonSymbolDeclarations declaration = UdonSymbolDeclarations.Local)
        {
            IncrementNameCounter(type, null, out var counter);

            var u = GetSymbolSignature(counter, type, null, declaration);
            var s = new UdonSymbol(type, u, null, declaration, "null", null);
            AddNewSymbol(s);

            return s;
        }

        public UdonSymbol? GetUnnamedSymbol(string type, UdonSymbolDeclarations declaration = UdonSymbolDeclarations.Local)
        {
            if (TryGetSymbol(type, null, null, declaration, out var symbol))
                return symbol;
            return null;
        }

        public void AddNewSymbol(UdonSymbol symbol)
        {
            _contextReferenceableSymbols.Add(symbol);
            _contextDefinedSymbols.Add(symbol);
        }

        public void AssociateWithSymbol(UdonSymbol udon, ISymbol symbol)
        {
            _associatedSymbols.Add(symbol, udon);
        }

        public UdonSymbol? GetAssociatedSymbol(ISymbol symbol)
        {
            if (_associatedSymbols.ContainsKey(symbol))
                return _associatedSymbols[symbol];
            return null;
        }


        private bool TryGetGlobalSymbol(string type, string? name, object? value, UdonSymbolDeclarations declaration, [NotNullWhen(true)] out UdonSymbol? symbol)
        {
            return _rootSymbolTable.TryGetSymbol(type, name, value, declaration, out symbol);
        }

        private bool TryGetSymbol(string type, string? name, object? value, UdonSymbolDeclarations declaration, [NotNullWhen(true)] out UdonSymbol? symbol)
        {
            if (_contextReferenceableSymbols.Any(w => w.Type == type && w.OriginalName == name && w.HeapInitialValue?.Equals(value) == true && w.Declaration == declaration))
            {
                symbol = _contextReferenceableSymbols.First(w => w.Type == type && w.OriginalName == name && w.HeapInitialValue?.Equals(value) == true && w.Declaration == declaration);
                return true;
            }

            symbol = null;
            return false;
        }

        private void IncrementNameCounter(string type, string? name, out uint counter)
        {
            var signature = $"{type}_{name ?? "_unnamed_"}";
            if (_rootSymbolTable._counter.ContainsKey(signature))
            {
                counter = ++_rootSymbolTable._counter[signature];
                return;
            }

            _rootSymbolTable._counter.Add(signature, 0);
            counter = 0;
        }

        private string GetSymbolSignature(uint counter, string type, string? name, UdonSymbolDeclarations declarations)
        {
            return declarations switch
            {
                UdonSymbolDeclarations.Public => $"__{counter}_{name ?? "unnamed"}_{type}",
                UdonSymbolDeclarations.Private => $"__{counter}_private_{name ?? "unnamed"}_{type}",
                UdonSymbolDeclarations.Local => $"__{counter}_variable_{name ?? "unnamed"}_{type}",
                UdonSymbolDeclarations.This => $"__{counter}_this_{name ?? "unnamed"}_{type}",
                UdonSymbolDeclarations.MethodParameter => $"__{counter}_method_parameter_{name ?? "unnamed"}_{type}",
                UdonSymbolDeclarations.PublicConstant => $"__{counter}_public_const_{name ?? "unnamed"}_{type}",
                UdonSymbolDeclarations.PrivateConstant => $"__{counter}_private_const_{name ?? "unnamed"}_{type}",
                UdonSymbolDeclarations.PropertyBackingField => $"__{counter}_k_backing_field_{name ?? "unnamed"}_{type}",
                _ => throw new ArgumentOutOfRangeException(nameof(declarations), declarations, null)
            };
        }
    }
}