using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Extensions;

namespace SharpX.Compiler.Udon.Models.Captures
{
    internal class TypeDeclarationCapture
    {
        private readonly CapturedAs _captured;
        private readonly TypeInfo? _info;
        private readonly SemanticModel _model;
        private readonly ITypeSymbol? _symbol;

        private TypeDeclarationCapture(TypeInfo info, SemanticModel model, CapturedAs captured)
        {
            _info = info;
            _model = model;
            _captured = captured;
        }

        private TypeDeclarationCapture(ITypeSymbol symbol, SemanticModel model, CapturedAs captured)
        {
            _symbol = symbol;
            _model = model;
            _captured = captured;
        }

        public static TypeDeclarationCapture Capture(TypeSyntax node, SemanticModel model)
        {
            var info = model.GetTypeInfo(node);
            if (info.Type != null)
                return new TypeDeclarationCapture(info, model, CapturedAs.Info);

            var symbol = model.GetDeclaredSymbol(node);
            if (symbol is ITypeSymbol s1)
                return new TypeDeclarationCapture(s1, model, CapturedAs.Symbol);

            var symbolInfo = model.GetSymbolInfo(node);
            if (symbolInfo.Symbol is ITypeSymbol s2)
                return new TypeDeclarationCapture(s2, model, CapturedAs.Symbol);

            return new TypeDeclarationCapture(info, model, CapturedAs.Unknown);
        }

        public static TypeDeclarationCapture Capture(TypeInfo info, SemanticModel model)
        {
            return new TypeDeclarationCapture(info, model, CapturedAs.Info);
        }

        public static TypeDeclarationCapture Capture(ITypeSymbol symbol, SemanticModel model)
        {
            return new TypeDeclarationCapture(symbol, model, CapturedAs.Symbol);
        }

        public static TypeDeclarationCapture Capture(IParameterSymbol symbol, SemanticModel model)
        {
            return new TypeDeclarationCapture(symbol.Type, model, CapturedAs.Symbol);
        }

        public bool IsVoid()
        {
            var symbol = _captured switch
            {
                CapturedAs.Info => _info!.Value.Type as INamedTypeSymbol,
                CapturedAs.Symbol => _symbol! as INamedTypeSymbol,
                _ => throw new ArgumentOutOfRangeException()
            };

            return symbol?.Name == "Void";
        }

        public bool HasValidType()
        {
            var symbol = _captured switch
            {
                CapturedAs.Info => _info!.Value.Type,
                CapturedAs.Symbol => _symbol!,
                _ => throw new ArgumentOutOfRangeException()
            };

            return UdonNodeResolver.Instance.IsValidType(symbol!, _model);
        }

        public string GetActualName()
        {
            var symbol = _captured switch
            {
                CapturedAs.Info => _info!.Value.Type as INamedTypeSymbol,
                CapturedAs.Symbol => _symbol! as INamedTypeSymbol,
                _ => throw new ArgumentOutOfRangeException()
            };

            // enums
            if (symbol?.EnumUnderlyingType != null)
                return "int";
            if (symbol?.Name == "Void")
                return "void";

            // arrays
            var array = _captured switch
            {
                CapturedAs.Info => _info!.Value.Type as IArrayTypeSymbol,
                CapturedAs.Symbol => _symbol! as IArrayTypeSymbol,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (array != null)
                return $"{Capture(array.ElementType, _model).GetActualName()}[]";

            return symbol?.Name ?? string.Empty;
        }

        public string GetUdonName()
        {
            var symbol = _captured switch
            {
                CapturedAs.Info => _info!.Value.Type,
                CapturedAs.Symbol => _symbol!,
                _ => throw new ArgumentOutOfRangeException()
            };

            return UdonNodeResolver.Instance.GetUdonTypeName(symbol!, _model);
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            return GetAttribute<T>() != null;
        }

        public T? GetAttribute<T>() where T : Attribute
        {
            return _captured switch
            {
                CapturedAs.Info => _info!.Value.Type.GetAttribute<T>(_model),
                CapturedAs.Symbol => _symbol.GetAttribute<T>(_model),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private enum CapturedAs
        {
            Info,

            Symbol,

            Unknown
        }
    }
}