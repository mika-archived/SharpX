using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Extensions;
using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Captures
{
    internal class TypeDeclarationCapture
    {
        private static readonly ImmutableDictionary<SpecialType, string> CSharpPrimitiveToShaderPrimitive = new Dictionary<SpecialType, string>
        {
            { SpecialType.System_Void , "void"},
            { SpecialType.System_Boolean , "bool"},
            { SpecialType.System_Int32, "int"},
            { SpecialType.System_Single, "float"},
        }.ToImmutableDictionary();

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

        public ImmutableArray<MethodSymbolCapture> GetConstructors()
        {
            var s = _captured == CapturedAs.Info ? _info!.Value.Type : _symbol;
            if (s is INamedTypeSymbol t)
                return t.Constructors.Select(w => new MethodSymbolCapture(w, _model)).ToImmutableArray();

            return ImmutableArray<MethodSymbolCapture>.Empty;
        }

        public bool HasValidType()
        {
            return GetActualName() != "void /* UNKNOWN */";
        }

        public string GetActualName()
        {
            var symbol = _captured switch
            {
                CapturedAs.Info => _info!.Value.Type as INamedTypeSymbol,
                CapturedAs.Symbol => _symbol! as INamedTypeSymbol,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (HasAttribute<ComponentAttribute>())
            {
                var attr = GetAttribute<ComponentAttribute>();
                if (attr != null && attr.IsValidName())
                    return attr.Name;
                if (attr != null && attr.HasGenericName())
                {
                    var generics = _captured switch
                    {
                        CapturedAs.Info => _info!.Value.Type as INamedTypeSymbol,
                        CapturedAs.Symbol => _symbol! as INamedTypeSymbol,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    if (generics != null)
                        return attr.GetActualName(Capture(generics.TypeArguments.First(), _model).GetActualName());
                }

                if (attr != null && string.IsNullOrWhiteSpace(attr.Name))
                    return symbol?.Name ?? "void /* UNKNOWN */";
            }


            // enums
            if (symbol?.EnumUnderlyingType != null)
                return "int";

            // arrays
            var array = _captured switch
            {
                CapturedAs.Info => _info!.Value.Type as IArrayTypeSymbol,
                CapturedAs.Symbol => _symbol! as IArrayTypeSymbol,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (array != null)
                return $"{Capture(array.ElementType, _model).GetActualName()}";

            if (CSharpPrimitiveToShaderPrimitive.ContainsKey(symbol?.SpecialType ?? SpecialType.None))
                return CSharpPrimitiveToShaderPrimitive[symbol!.SpecialType];

            return "float /* UNKNOWN */";
        }

        public bool IsArray()
        {
            var array = _captured switch
            {
                CapturedAs.Info => _info!.Value.Type as IArrayTypeSymbol,
                CapturedAs.Symbol => _symbol! as IArrayTypeSymbol,
                _ => throw new ArgumentOutOfRangeException()
            };

            return array != null;
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